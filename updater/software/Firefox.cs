﻿/*
    This file is part of the updater command line interface.
    Copyright (C) 2017, 2018, 2020, 2021, 2022, 2023  Dirk Stolle

    This program is free software: you can redistribute it and/or modify
    it under the terms of the GNU General Public License as published by
    the Free Software Foundation, either version 3 of the License, or
    (at your option) any later version.

    This program is distributed in the hope that it will be useful,
    but WITHOUT ANY WARRANTY; without even the implied warranty of
    MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.  See the
    GNU General Public License for more details.

    You should have received a copy of the GNU General Public License
    along with this program.  If not, see <http://www.gnu.org/licenses/>.
*/

using System;
using System.Collections.Generic;
using System.Net;
using System.Net.Http;
using System.Text.RegularExpressions;
using updater.data;

namespace updater.software
{
    /// <summary>
    /// Firefox, release channel
    /// </summary>
    public class Firefox : NoPreUpdateProcessSoftware
    {
        /// <summary>
        /// NLog.Logger for Firefox class
        /// </summary>
        private static readonly NLog.Logger logger = NLog.LogManager.GetLogger(typeof(Firefox).FullName);


        /// <summary>
        /// publisher name for signed executables of Firefox ESR
        /// </summary>
        private const string publisherX509 = "CN=Mozilla Corporation, OU=Firefox Engineering Operations, O=Mozilla Corporation, L=Mountain View, S=California, C=US";


        /// <summary>
        /// expiration date of certificate
        /// </summary>
        private static readonly DateTime certificateExpiration = new(2024, 6, 19, 23, 59, 59, DateTimeKind.Utc);


        /// <summary>
        /// constructor with language code
        /// </summary>
        /// <param name="langCode">the language code for the Firefox software,
        /// e.g. "de" for German,  "en-GB" for British English, "fr" for French, etc.</param>
        /// <param name="autoGetNewer">whether to automatically get
        /// newer information about the software when calling the info() method</param>
        public Firefox(string langCode, bool autoGetNewer)
            : base(autoGetNewer)
        {
            if (string.IsNullOrWhiteSpace(langCode))
            {
                logger.Error("The language code must not be null, empty or whitespace!");
                throw new ArgumentNullException(nameof(langCode), "The language code must not be null, empty or whitespace!");
            }
            languageCode = langCode.Trim();
            var d32 = knownChecksums32Bit();
            var d64 = knownChecksums64Bit();
            if (!d32.ContainsKey(languageCode))
            {
                logger.Error("The string '" + langCode + "' does not represent a valid language code!");
                throw new ArgumentOutOfRangeException(nameof(langCode), "The string '" + langCode + "' does not represent a valid language code!");
            }
            if (!d64.ContainsKey(languageCode))
            {
                logger.Error("The string '" + langCode + "' does not represent a valid language code!");
                throw new ArgumentOutOfRangeException(nameof(langCode), "The string '" + langCode + "' does not represent a valid language code!");
            }
            checksum32Bit = d32[languageCode];
            checksum64Bit = d64[languageCode];
        }


        /// <summary>
        /// Gets a dictionary with the known checksums for the installers (key: language, value: checksum).
        /// </summary>
        /// <returns>Returns a dictionary where keys are the language codes and values are the associated checksums.</returns>
        private static Dictionary<string, string> knownChecksums32Bit()
        {
            // These are the checksums for Windows 32 bit installers from
            // https://ftp.mozilla.org/pub/firefox/releases/117.0.1/SHA512SUMS
            return new Dictionary<string, string>(100)
            {
                { "ach", "94e35e1fcdceab158014ce461072bf7b1b895fd1fa9d5930d0fac6b4b7c9b69689ebc8b8c8783b0133d9734bf233225f78bc0319a32ba8914ee0634a3ab1267c" },
                { "af", "de8046a47a25a58d65f3f164f474b6a7aad305cc858693965ea00044aed58c8d72c2e328e5d1d2b72045d33bac029efc7b0139e6167b9aa83bd77379552f9cc2" },
                { "an", "52b2c87d9657b6137a9f57caed0e58dc49a80a40c3d7c2b650b576e7bab500c6b67cc9b94212c44b6ae03de121ef4d8c7f9b587fdce9a02bc1e7f1f4a9df7583" },
                { "ar", "1a751f77ffdf844b30d8195fe8cabe729d604f4603661e9b33e949aafc5f7b9bfc2ab151a2fb6100ffc68f2132075d701ed68c6642ecf6a6072577d8eeaef845" },
                { "ast", "feef2125b1754fdb4eb90006ac12d1f8af839c95f8019d6b352ec501e3071bceeee608d89387cb0bbb34e32d3c75b112746e0c3c7831f4fbca7c0bf53cce5e82" },
                { "az", "1112d4acc3141e56dc19d0d9bcb07f6d377d350d9d8a72a5ab3cffd9628bcf3f1a5c71abdf2894a98211497c7abeab1343e5d59185e4bb793280ecaa3f2a9246" },
                { "be", "92f3f9a0500a27f78b85999a8ed4695736eff56d12cca48d4995b6122a069b01b2dcddf0ffb89c767176600b4fd185dd54c5f050239f3967bcae13fd9763b806" },
                { "bg", "0532e83b4fe41be128d314ef9aff7354d3fde2a385ae327d278c7ef2aa9ba6d238ac4ca4d1f9a6bded82063c3d3f30dfff6ce0330516a06d5ebd5a52d1f6e407" },
                { "bn", "8b3291e1e3e32433b5f2c6842b02c55c59e9961569ae595b8f155c925013aacd37b0cfe29e9a9445fe4a01caa6328fc8c380219e8fb8176f03522fbd831b1a98" },
                { "br", "3aa81833176406edaa78055349cc455d767931067d311198aa33e1a18a5bebfbd9785b7bf048614055f992759811c7dcfa109a21765b9644fffd0a88f6be560a" },
                { "bs", "9757ad59939a063a8fc63af111ecee1a4c5eb491988d6ccda84d0d45ebc4c093276d87889de10a1dda77d119fcd132414d92dc7cd8e724d6ee7a3147d9fe1d9a" },
                { "ca", "2980d0400dd38c0aff7bec424437d2fb0cda07043fdd30cce9032fdcd7c6c39991b603dda37bd07befa9e230d545af84ffce5243cfec6ae3ee86c84994721afc" },
                { "cak", "03447df3d0f3e3710f9a6b71406211aaf703b853e00b34683c3e2ed48c0a680cb27e573a176043687589ffd2a188fd237271bb868f26eae0846831ac079769eb" },
                { "cs", "d37c50b7d56949e4986c30920a9e2826e0f45ba04ae08652b7edea36c6246d1943a12de480b31e467ac92e1e8e0c867fc24d07a522006e96dccaf20117462351" },
                { "cy", "2a1fadf61a6c61adda74733396a4cff56da12cc4e5f50e368a08ec600de1270a5a090c6f0f445bdbf793d71a76963f981ea894c49da9d1441910e63547082c20" },
                { "da", "409ea155136a8a9c834187da1f76b9ae8ade4c04e9335f5269b2dfbc5d67552cb3d0fb2353167aa266d7423ecd4dc203b42f3e3d832e7bff6f9758a85984e3b8" },
                { "de", "3852a26a697e33b132c7ff0ffaed3fb5b05015942436cf9a8cfc194a17bd6107e9103df70548eb679ffa16989d7c2ed5439586321a64848553e42f4d05db66bb" },
                { "dsb", "88db2ab4841545ae0d55271b4e7f0856b0571eceecb56f2ae15b3968845a1707b89f19d8c3df374ab585a686f6880a723a999bf2d5199784f28250c9ab500573" },
                { "el", "11ea365ffe2e507f636f9f06d52f31e21a62ef792741ed5c264b5a7a06b460a59e7f8b5092fd10e22ab166c7fa4791cf52d8377c9671c8fde14225d76c1e0971" },
                { "en-CA", "6742f9611d79872f29435638b144c019978e3bd099a9e408e4519fce657f19962a6ae05ca6e9485cba443fce232cb7131f9c08f2681180493494ea7b3a13227a" },
                { "en-GB", "96cea572ca69236cf9359b98209ce077ca2f6b60c4aefb160125d83246b8ae733b69811c35393b7f56ab72f88c90cc94361b7a075778b65d978a2c65a606c1e2" },
                { "en-US", "5c93cb3262b9d70f4f35e5edec24215de8929b224b1c3cb707d13d158893bacc1acdda063cb859f395efa90b6d3899329d963ce2f21f860238b472f33bc0d72a" },
                { "eo", "7f352f1bb94728fab75257c9e0bbaf3ed2bcd5819b761f2916febab17dc2b62df66b853bc35cc9b2bfe34a7e63c2aeeeb7fe36b8c4c29a379341898eb987090e" },
                { "es-AR", "e7a1e372ae7dc2c339cb4db3327bd234b4e4d8b88d636eb203c7e8cb1f72e59e5f60853573c93799c05985e199c0d1b2597677421bf8f3e255efef3929e34df2" },
                { "es-CL", "9196300c33a05880e71d4587b0a6f1eb7b3ce422c69e3d030ced75c8a76dc6d9eae57c3bd53b4edade2c1841c978745491e5f629c8e92c1332076584b5d65e4e" },
                { "es-ES", "ab983c180a30255cc4abd1edf6df3abbe519e4e883a4c3462bae031ce23d9571280fd37d17069f571c4d875fc5798ff18990aae4a1956e6c71c1c800faf4d862" },
                { "es-MX", "1a895ba8b5310fb7070e19c74a14fce142da69275a8795d5f07cfee0b6f0ddfc25d8fd91426e77c4dc4534de0f223bb72b7979f67e41ef7e23a38b7d3f366b00" },
                { "et", "bd2c1927291d26842cde9ba75e89ec676504416a43ef1380ea1c41c838cd0db4b28be4598ce3a3adfa6d8715dc306c29e22bcff3533460ed9941599284147adc" },
                { "eu", "ef1a4b09fb6d9b4e8c3072a8b0e0e8e219f3df2329732ebf6c4e12f6e6d34a490fb0cfd44ab50c2ba686b56552c015b69fbd89bbfa09433cb9f1a2d6c9dfc2d2" },
                { "fa", "37b1c39df92322cc3f4319d2b5a0074f30ef6c7ab4615f7f083b1ea0b6d0a03130bda8ebacb2bc9c4be0bb74b6a764f03d33960d359d26bce259a8b02349fe7b" },
                { "ff", "b065e86121f10eb45765cd939ab67472c9b800d9f6941821b9b1c5ada2fb33dc735cf476292d189be7e693ebbb047b524a7243c521161c08dd9b20205082aea3" },
                { "fi", "ee0c1667c3e5478301e9e1a3b6c189b152ebce41c94286a7eba7375a3d0662561ee4292bc1a0158e16ef0946839c73f4e7c64987ac508c90d3aac3561dd1098b" },
                { "fr", "18776584e384fcf2d1ab5c04db30b9e57781c7a2b8d38967931aa4b08aa4cc7ba44844d46fe0a746acba3b469127e5e7b26504c092eb8aba6b5ae68c6123e134" },
                { "fur", "63e39416eb3ec78a8204256d61fac8aec7ecf0d3f12cdc7d4ccacc9b1b36d639f68665b83973f3cdc0e8853ae763b1faee793299044acdd84b4b1c4c347393bb" },
                { "fy-NL", "20511fba2449e4b4a8206a1c9ca84802cc8dd23a6e8b063ce64c26d0782de531ec5335a6e5325044b665121025ebcf818899c7af8a95dad198a3459178badf6d" },
                { "ga-IE", "966efbb3176caf605b6a4710de0b6c315c671c25a7edb97135702b9296aa84fd81fea562d271fce66ae78472ff8c98d6ca404d6b3ecf4bde5ea471410b4b17c8" },
                { "gd", "5e99e5aae2d995790a2b37b75ab5cae770c78b63bc24f8f10dde81b11d20ff02a7ac253103dfdad596ffcce075c91c2c91c3c9f4e309256ed3d92d6f4860a907" },
                { "gl", "814f3a9ede6221b083d2f66dab11d9d2e736e965bec00091ef2b473a731e7d591a786a67e5bc4acad01cadbc4757001405d2a9046f2089aaf06726117b83cc60" },
                { "gn", "b94f5f1611e05fecd3d2ef0456cf1a434eed248a0c87cfb0faa578d7ecf41ea84320c20490b8880fc806c672601377546301efa4981939a982b443bf2626bc1a" },
                { "gu-IN", "18bb6cec2cb49993ee5e0018871a6594dad2a37f7fd4850f6aaaf4736a824fc65719200e771972e720bd3c9349df7352909cdfe31967913ca855f00f1e226a18" },
                { "he", "a4f1eb81b84f8b6a8cefd12155aa9839ea4f251ddbe28f22726357faa1ea94ec3f50c3cf406700a5f906b10fc952dc2476d1b1dad9c1e6b3abd476ea731a2dcd" },
                { "hi-IN", "b04d299f397a420e2783c79aa683ece14514380c2ab2c63faa6d407c1183f903f3172a5fc735f099100c2d25c6d58d5be0b8b8302180cdc863747c7f83a0b041" },
                { "hr", "21c6155ff01f19355f5ab22732bf89cebfb525b2899896431f85b52ebde7ec195979b45ebae62fe568223e5f574076f972706d33132402ec776d8d55eb256b24" },
                { "hsb", "500b1111a2507ac3ceca1504d38d610a37b0a8d230349d6936c7d3abdb9cd33f0612622ea78aea2a1d35fc5a25fbb7b2d53922520e7cabc3f5826ef96d1050b4" },
                { "hu", "de582d19b0b61affeea5a44246705a5b3f47a95199213b00ccc73223f3f4439db6f6c2f865b4bc240a3a8a9ea82cd4d64cc63114882fd196fb92653852680d34" },
                { "hy-AM", "51fd7c25ddba68ebac3425bf705661ff08d82007a73f5f40ea3356f4ab6acf507860cf18295a96c1d41517aa6953d110f62f5dcf0278b77951053a2729aabb5e" },
                { "ia", "03638459bcde20b69f010c6e86296b1279fb412ea0fbcfc6a188fbf3c267ec53d35689df9aa6eeb80ec5fbed20bac4f4b54bd32d00ee175132bb71ee3c77f4c5" },
                { "id", "9b583215687ca944023e7895aa5c00335f4cfd0278b1ef693132cc609a931c826e3f0571790edb25eaad6cadec0555627481a063ee530e9ccbd38320c8a5c4d3" },
                { "is", "bd73966c419c1c0e32c1396666c67b03fa427a746a24f976f61f760831d453b8f73df63694c4aae66836c2c81eb93e7110bcb94705e512d7438dd608651fc689" },
                { "it", "ab1563c053443e5b22cae511b726a9013e2091de881e8ac14ceeb3770fff7699d2b40d9219cad870e6ba1665ac097790609628c53c7ad1ac25fad3bf738ed4c5" },
                { "ja", "058c9d30c188ca520d012c47671537feeea1853edda5fa4a9897d77b05edd51a95ce7a39dab961712197d28e053bd9db51d2c98078e702f0554bb8a6b2c20c31" },
                { "ka", "ff5f1dbdfbee2242b70ecd2df3bfdcf7cbfb017d5ec2bdbe3cbfdb20d10735941ddf660cded6c86a2fa81185d757feaa05a8318baaf6e6b78be26b1e1a54acfe" },
                { "kab", "725b33f3a8c95fb47c43f524619c7adb881ff81a6728f6d53682cc5252e035508a3f654d3b4d6ecdad9ee29b91dc2f7b20388af06c696d29464140dd9b1f77a8" },
                { "kk", "47eada775da2d84b75469e13447b55835e32a7b07b0366da59ab0e1ef822c74b451bcfdd48de810b4cbe19162cf643e88035b40bbffbe1a68c6a4dfc3df09c0c" },
                { "km", "3ffff27e7798b081a39c8bd4e1a5e1afbb4e5be605a0b640048cd15d5944ee593f25f1b29fae1127f97224b3d9b8701b325481da0619849120418da767b7a7bb" },
                { "kn", "7c6d260b977e32f6658094f6389b08e5ef208bcad97317a78d8b6770cb0e71557d5834e32459425282fa4a368f0a525f83e6687b45464b3443c855a6581f95ff" },
                { "ko", "6e8f9f9eb9973da94c0aa06338e7d5735dedabc307c0cbe660d184f284445d03f7f5c9aa8aae30c53f8a4ab55738facb6200d525606485b1f7a70d8a488970a9" },
                { "lij", "6e3b355c94673e05aa64ef9ab788a381fd5c89ff974f50266669c5971b685a541671f9bfafc854df46da4e512e1607aefafb338dd4ef1c17c2d9cdd4bd09b971" },
                { "lt", "16bbbcd1ef6db16f3891ab8fa9dcd5479a6c7266ecd70c0766376a5137016c3c7e3b782fef670e64b9b541bc2e61e0c3e217190f6879db971692f5edd3fa581f" },
                { "lv", "d0db40233980e36d97d89ca08b223adce0aae03373f1eed529638eb743badead74baa9137a9869890d916c95498a6f6c6fb5f8a08c46a3dcaf6cdc758446dffc" },
                { "mk", "2c9823cc547a9a6b0510f6c0706178f8728a491227818a7ff689b8266778a800216ac89e15bae9a20b61a50ec5143429aabc25bda9b8c2f1c9bfcc22c256370a" },
                { "mr", "80cceeca39e13fa5425d7922c75836eaad093a1b893de7c91a53af914fb550f724bfd6d6f5254d004e02b9fc54ce1ee2aa323f92fcf80f63884f621b2037172d" },
                { "ms", "35dea8ad84616816a33a5cd58e708c960b22cf4957968d831741d2aa4e013efd0a05d603f4d54ea1916898dda9e4eea4d2084dc30a4d4f845040217baa6a95e6" },
                { "my", "d652443392ff71766014ec2350fc4df977b6130739af41f8660054c8a2610d49bd5dc232b060a5c1d2fe73bf64267522a4b1981c9e546adc3715c83eaf2fe563" },
                { "nb-NO", "a34b24a43f71c8dac73616af8e439e13c0d764ec0ff1b09a4eda4ccd88c69b2861010237c9bfc7b4bfade3257bd42a79b421592adb34060b28c9974181071dee" },
                { "ne-NP", "7fc4c4113e064dd4b49f7834ae33585023d29a5f06bc24714d8824d868d2c21a7535118ae074df631af2ffbb6dd263ad54f0bf9b8eb741b7d10054bf20eace7a" },
                { "nl", "873d33d71d205520df9c3ff4a933477f05a7cf917db760b3285a3fc1f1a562605236b5b3043bd12069661b7e80fd0f21e5a6adbe060ccbc7c2989bdcd0959331" },
                { "nn-NO", "081f84bf04c25f068e0ef2e972194aa3292a55dc2215ec6b3865c687af2a85875c50811a112166d43b1213ca15b5dc43f21fb3eb8aecd3041e7835f6f2922d88" },
                { "oc", "a098e69a9ea4769e9e70ff4059910f8c84efe42c55d8af97801db8ca35fe1fe34a8132439f8e17dc2cd81ec1b1ecb31ef279e68a9b47acf620f7f5339366b5c9" },
                { "pa-IN", "54bce2d0b5f48091ed81c4d32d6bbfe45fc1b57c07e68985ba55ff06419cccd2ba089d1bb4f31777d174f1dfb501cefaf5ab8fcdd0e37bdbeba0de22a1694b99" },
                { "pl", "00a03da7fa730bcbef8f033baf003de5c45a6f003e47641f05b76f109186fd882b22778e9178f1eec2541931727551816a5a3441dae92064b7a0b34d026b1eb7" },
                { "pt-BR", "96207f7ebb951569311c33fe6dfaaf4d39f7f60dc2c7a02b945ac516c40d4f80a9be47b347bf5b18c078d8488f759bb3763c88e38e6baf835153475c0d1e81e4" },
                { "pt-PT", "fdcba3eb2a075d61614bacb47d7b829ea9a0a44fb6ae95c79bc18fdd9c8fcd2b5098a13ea3dd138afb270a56e230099fc85934795e3230bba58f8be620fd9299" },
                { "rm", "f3250bd233cc274218256672d825fa5ce44887d712c8b749d71b00237d4efb13bd2ccd00b50e2be25e5cd59b1b9a1d483a7f55c6b413bf4b0b967e5b9be5be91" },
                { "ro", "1e48066e90bbec487c56546774013ee7f9576556b4d629b09ba1dcae8161b2d32d1fc13acbde0fb1e04f8614da61f8f52e315f8805833c9895330120d2f4360d" },
                { "ru", "1a536716b65ccd2132c5d5b52aa108d0990a96b546a315eed81e689fadc3119486108d762b1de780e86df57016c2248c74b35215145ed48e2a14d68296bf5285" },
                { "sc", "fd1ea726b3b75997528d06dad4e3b0d0b4c9776d0a3b775755f60dab6a1d27be4358c0037c19aa6873dc654b74d0fb19983d95b49026c3168bfec994c57c4ae6" },
                { "sco", "24852e4c5a94d05f8ca596612c48167a9401a87fb79936732550198f26c4fa1b2f60f965605a6cb0bcb299b95bacaee9439c1aeab8313b38aa025c686a86da3f" },
                { "si", "1f869cabf80ff9b6a094d1c32bfeffb660b7236fcb9b408c2661236ae72060c8568e28bc364578daef7624a172caf8796fa185e0e7808b9886562aa4d0e2c472" },
                { "sk", "5aadcad9807d974d2a9d4e9869702362bfd34700f1f10602e70688b8d4c7b5ba6d66b69f360c2856637f0c6d4fa5b7a47ed69d02fd77af62f1115bc685ad8731" },
                { "sl", "c6616f1c212f2671fb5236d80a6b3121d12572073c493199b77d6c10af6b9c5099d1a70e7cec7f1f69c9fbc103d8b2a823d23b4c119859ff9eca2218ddd459a4" },
                { "son", "47e3349f96d289bf8aaeb801e75d275a16f963f3dcbba457eaa120ce24a1600cd6a6ed008ba11d9cebc0b443c65ec257f409e62dace2498fc07c7deb27bfc674" },
                { "sq", "d76550ef9ddccec5aa0d9874af98a4ef2bbf2525858dd1ed001d085a2f70c2a08264d9d254b759e68e1ad1dd9a8ed8cfa335845ea54a0a8231c04b2fe3d70dc5" },
                { "sr", "4c293def49b85741c44b77602c54d10bbd283a891cdd9214c58e12ebca0e0fc22e8291ff4e315ea1c4f4a46f210f2b44522716fe5f71670a703d48dd85ab4460" },
                { "sv-SE", "e785e7b6212ffadff52c4e7da297e46cc4636eddf595e1ac23946854d94ac7866a5a702b998a8435a20a174a6c66eb25ee20340db73c51a0dd1534f2513c6f80" },
                { "szl", "2f2219225442065637f2c1c3393b30eb2dda20c7000e9c5c315ef9c3e8a957605ec1ea52a3a2b29bebadda560deb6ac07482bc54f10dab61275f982aa2e65413" },
                { "ta", "06c59009b8e9723778c398b3ae0eb841efe67ecf5df95f6c046c87ccdb48d62d8b67bc8877db311d6375cedb6320ff3c5e450149169254affb26e52a985a5c5b" },
                { "te", "cf454410dda6aef4640db10e4d653ea6f256688cf93a68cb38e9bfb4dda5a699b871af62519a82be0b9839d2b62be35714cedc0fbef89e789f36bec4805da4c1" },
                { "tg", "87cc6d87bf402cd06945355c05179735ee6cf07019de58b206e7a47039a386aa1ddfaa1030e89154f3c2e532502333dbf355b8bb8e379fef7351edfa577252fc" },
                { "th", "c4b2abfda2338c0561ec38e52fd5be5485f9688f7c1ac1c0ed3ad19e813cdaf10679bfc04b77cfb471a59b1fc8356363d0b457361e81fcf1509bd115cd639a1b" },
                { "tl", "692d0863673b0a72acffc456798c363da92eb67931b7344044f226df78016db4957ed15bab9bc4a2c5d98de28b5baa930d96b88d21f5801822e9362182d68393" },
                { "tr", "49c27d3c0b7d181896c9458c7cec3e6b2f72e62e393347660ea7b5c7bcf80d390eaae1a9adc1def6044384e0561d7829cb3881542274b455b116dc769ad2e395" },
                { "trs", "b5531e5dba475693344ee3440d10fbb327f0ebd29b03554c3c59139f7f68e70086f0417acb0d6d55b9b65f2fd75b9cd184b341dc4fab11ca015ab351eab729f3" },
                { "uk", "27f60443ffa4ff9c83edd4e275ccdb60b4589ed2da5f63379ad5c997b45ab0c2445bf7ed30ef8255a6108cf8c617e2763b0a421b42833f95347e81fb22b568f1" },
                { "ur", "6399ec82019936bd4567754b942990e1e6b06e61154cadddb7dfa3a37512a109fd510e0fe569bc79384b050af4bc7649165b46c5d7f733c7986338d55bd121bb" },
                { "uz", "34637311ad26ba0579955358c1990b7f6b74d43c7b602d6fa7dc858ef37a2cbdb1a4b85c5656c85bab792ee5ff12544974411f0ada15cf819c477967f3be46d5" },
                { "vi", "9a032592886409c3b36c737512a375d4281c977cccb72c6dd73fa24fccaa9170896bd61256155bbdce3bc80428bb51bc403b1879db7070e58deea3f52640298a" },
                { "xh", "f5a11d13d042d2b5ba586f0394256256906657833d99432907125dee5339ad7c5fb4e3f4af13ade2523fe060395305fb594d23b8206bf0b15a82fbfc6df24f49" },
                { "zh-CN", "72589831e5b40dde19c79eece1f77bcd086eae0c1078aa077cc8e9b702baef390e2923f712943b6fbb3ac366b792b38d8053c20e74068999bd7aa0abb7b84613" },
                { "zh-TW", "5fac58c3b11494a4f01790e6840748f346083fac9b715f53e5f47c190768f3a02e28c53a2006d7b2ae9d87349fa48f96fa18277e7b80a56366ebed59fd791e80" }
            };
        }


        /// <summary>
        /// Gets a dictionary with the known checksums for the installers (key: language, value: checksum).
        /// </summary>
        /// <returns>Returns a dictionary where keys are the language codes and values are the associated checksums.</returns>
        private static Dictionary<string, string> knownChecksums64Bit()
        {
            // These are the checksums for Windows 64 bit installers from
            // https://ftp.mozilla.org/pub/firefox/releases/117.0.1/SHA512SUMS
            return new Dictionary<string, string>(100)
            {
                { "ach", "ac8d0359d67814174ba517f2d585acb17adee8993e1bae0199d567b791d775592084113db20f294f4394488d87469f5694b052cf300c9bb50e1386f970eff4c0" },
                { "af", "d01d3c6cf4bb21e3a8c08226b469ad9cd8619a27b94a76bbc53461057af7749ea037b4fa654ae06befc90139361690be4e1e943bf3558b8e54564f4316ce61ec" },
                { "an", "c54e21f9ba494b9873d14151816f85574867b58ad1d78fe80e6f194f99595234328460fe65d4bdc4cd2fee2c7fc60191ea95443886b98384c64065ff59c6ddf8" },
                { "ar", "e5bfd755b8ccdf064201436a26da07d74edcb6c0d6d9b1e341c72604aa3efa2c47894cd86180ac4371f4c2e9e5381d983e7f5a20a23e3488a5e9e1ae98919103" },
                { "ast", "4342bfa3f1825aa1cbf7444121243ff58220e00f7e24992b5bdd219977e215ef31af41702ae52b32574404a68d7046d309c367360e9f59e567d53c67b695d10d" },
                { "az", "e5349cb3f289c76747602ad27607d05c10d50471179225a5464be1af8fb010a3054a2b5111f33615f2b4a0c814aa28b2eced40605cca0172cadddea3571ac330" },
                { "be", "4f8226ef41e941716da1f1c109a45f4c68104563cab9ee1d84f54fe325db22509e5fdd68e02435870f0b9558f547c922dcd58c0debaddbfef2b3cae6885ca8a5" },
                { "bg", "ad0cd9a5beca18c349e5d74814f6bb2de9e840bfdef9ae742102ee51207b49835632719a70097f628575afe571371f5d38cc0595fdf560a987b15eabea346ed4" },
                { "bn", "dcb2ce55ff737053d31d4d98723f9b87847d490f76602f97a0b4ee0aece8f8e537630373589ecfe50455ca7592f10882510444e26ecc8faede0873a321fc7f92" },
                { "br", "9f625fbd72ec348c3e94e346ec36eab314ad6e8cb3297aa3c7a19413c5cdb11b5a98f3851936e2745db091c590aecfd070a21e31fd64f96c0294e7a44c654e89" },
                { "bs", "1cda3d8a06493a8c846e833bdc51ab0ce7b7d0c21cfb7e0b88e6732d319847db3fa4e7e5d63f21d44bf05f820a463814271ec9451e46484add6fc3bfb5a0024e" },
                { "ca", "57dbf6b2ab9998b940500c2c8bedfb526e6f88af7a061a1eec55d0fec61e683c68d7ac828ad1ca86b36984cdc0d2cbdb616bdb74c9a46309b997c84076dab57d" },
                { "cak", "e7b6af2a63a0c502a310e76e4e90918d3b15ab930b087e3a8a9fdfcd5052d8acc2e1d41a3b6aa17b36b01a0d119ca8d39f67de1608b1fb91b97eecd67d115b33" },
                { "cs", "8e6cd4960c9721b271a46c18dca0c40263fc963eda26ffff496936bfecc7e35053aa265fb2fa93401a3d68583e550a10511b0915fccd5a353f3e4944559b9698" },
                { "cy", "a6b5a87e723736cbb6b69426725eb14ff4472bb4a35f5fac972fa3671c0082814c3e24034a1c04f4f917cc8c655abac039e377203ea512faa499aa6f546f80b0" },
                { "da", "259fb609db2377e31f0586fa4eb3fdaec1b1e8b12b5d535fcb49a3bd113b77e88e50ef0a63ec730699ca7441b51ee37fe2d205390cedb6d7e359008b980329ee" },
                { "de", "18810956bd0454108fea7ea954e86221cc48e39e4e469d41fc9d6705aa4f362fe1b0c6ad1787d906b0ec02a5a16a107425b187500b261734c248f76e526be969" },
                { "dsb", "08c8c47e0a9d1b545294cc7ba3335e3d3343082c1bb7ec83a7749003630a7a96419caa5302de959264856b892b3453c5830d1df75b33b641b7f390ae6bfe427f" },
                { "el", "27d51e3593b5245805cd6686a63a3b5bbe1b4cefddd0d3f6e735deddbef1dc43b387bc6a5a6da86e1fc9997208461c54768f443064632edd07486c9a6b59c5c0" },
                { "en-CA", "417bc57262ca246d577ab1d1d7278e9889fa61f019bae14a5c6cf58f83a1a15fd3926e0cef0f7ff6df254906259d88088c2d97852cdebf275eedc38e88ce8bed" },
                { "en-GB", "5e81cc542fad90df400d4245bf307efd8f8b0e8fd8568ffd01d961320a9cf0155983e9ced61fef3b21e51f9e285c53cdf63a1b5ae494e43a2358850befa3409a" },
                { "en-US", "4ec855ac8e2af9a9c68bafb07b95106b252411bfb22070d48660db585eb97e4558ffaab4475b841920dac810264c86fd2f39dce47c33295e4f040f03ff6d356b" },
                { "eo", "4c196617fd39454127cc57f22fc746dc7b02ddeb959b833c680a354657f9afa483f6f139eeff2902f024217e006ca94cfd300e3995557d2f9d65d47d7751a7c2" },
                { "es-AR", "e283db195fa24e01c36525d66f50568dd30434e95451406bb77bc15e6483d0f88d7fd62a5d2129aeda0975784b29f500ae5917c7e58ee3e5c7a8ea623eb4f511" },
                { "es-CL", "c250706ac0f32b7dd8048534ad3e69d769a8d0a19176aa07b822ffe0b3369885bec05e98148db2dbaf8c6eeb80587d4194a24cf526bd06220fe285f384f48494" },
                { "es-ES", "6e5684f086978430d2218145e1c6777274d710c88c68150bfa3ba7ac4959256da7be9be102ed26d03ff906a0bb89a500506c6b57c482d38219c4b8636dbe783e" },
                { "es-MX", "b00dfa6f4fca4c94c1bf320972bd5f38b8e3c007c06adb1fc8befde2c97339d0101835de032ce99165fe5418e7a30df2145fe707ebed066ca614a6deed618616" },
                { "et", "912c0e66bc866a8f5e0eb7e3da12cd8134442a250c7b22fe77469b35811454c334cfd92703625009a13390782d175633af6145bcb2bfc4c70d9ed43c51aa655c" },
                { "eu", "048898ab5a30ba32b2eb3753040f89265c342c594b24e013fbaca3ec4de0da00a3160cf2f244b4e122b1dfd1f655ed304c0a2952b68204e4d17b08064bf16877" },
                { "fa", "885216b25c9f3a231a4566481167a69503f8b4cba6c296bc8ff3dbd6a4dd3f32db72f4e7a91eda1fdd1e9e2eda231150c3a7535d1918d1e937ca0051c6e35bb5" },
                { "ff", "730faba862c2d9a012e851c3444f855ccb921b156384cda97b448fae41a752ed84069d040733f6b0ef31da731f27b95fbd55cda577bc2aed2e5f73a30ea274be" },
                { "fi", "6d31aa7b02f4dfb18c77551558ce2102142007bccd8b74c86ad191eb07ef02aecc354f60477e3e8380f73e34ff9cdd1334a4b87ac0bb53c0077851827064fabe" },
                { "fr", "4705aa22b6a24a0ac0b6f98edbd30b2343c59d5aeb10498abf427da4b60cad1af7d085d42c285bd1c2bd4fecf5dd7d9793c1ecfc88a7bea450b435d9d22e7061" },
                { "fur", "1c4bc7eb043b82896f771026865a5f8a8ebc244a3222fdde6f2ec23b7fa6baf29bbee2725680a87fdf005eb663a186de7b34cded9d747669f84cb04f07978507" },
                { "fy-NL", "3261f6968c2c4a1eaf36ffc673b7628351d90a240a826fcba11a7309cfeab36b05e4d39d8d190ba4bd624284a5cfce0941be80abdb39ffabaff7a694a0ddeceb" },
                { "ga-IE", "bc547bda1ea8801595e30a7bfd40826f5fcd38ed8fb7f17192b3f891a2566ef54fb09ea97313151f9278fe9f7b59b14a2aec57792b8ade49bd470ae7716d4562" },
                { "gd", "5aa07ed23593698b18207197b72d9300a1eaf957b3380430208b9f1ef39680ce0e500b3019d98bc18e97d8f5c31ac2db85bec2e5cddd54e96335211cead25842" },
                { "gl", "29bd099ddf9db8b5c2acea3963ba357a2d272a0e46b76b98999894cdd4a862d1f18515d41788e39542b1188150ce1ea6a915053a6a40662a749b599afc592229" },
                { "gn", "2c4d16da30c8d50b83e4291af02f7f0eae8e5331b520682048f86ed1cfe7d6a8670e1f8f85458c8233e6598c19cf65e5110ce824e241af0d535892545f8b1ea2" },
                { "gu-IN", "262d3de25812de6e04c7733655a380e73dae916ab1476dec8060712df0056b42991a0d12ea74cba1acab526a467d1580871a93f375713a8d4063e55b23fa8f60" },
                { "he", "1a27edf80fcbeef454106be534b34b765a5319315185b6ad6075faa5b5683a730fe304c63ae1e0bae2038d0b71fc9ac9881f30d21d58e6b56cc1ba2079c397c9" },
                { "hi-IN", "e1799ade2d54593c01a1e8fa16d8cb1b43195c4679dde93cda6ed14b947769ee268b0d8a385aa556a27d141abcabb19f5cf4c82a1b979ea2b6b81f2f731a6312" },
                { "hr", "24b7fe71fd6ce7ccc957e727de08d6337736e7527f72d6b7650a94773403e87ddf9122d10baafdbc4d791c02e8b89e847a499a0e36475991a058c09744a2741b" },
                { "hsb", "a88830697add97154c7b0f6629d44cfd46d5064257063994dab5225c3d064fb81331463a0744000cc3d43f8da5d6f17dbddfdef55fdd0a55a9befd62b23fb46e" },
                { "hu", "44f4c869828eac182d0467ded8f2d452b63a6577cde2f0dda7464d6f341f0abbd1ec2aaac1dcc776335beb00d9053d4577751c17dbeb8e604ac7ff0718fe8173" },
                { "hy-AM", "aba1eabb750f126bd01224626ed4c9119358f6dc486ed8302ad58deabe1359b14ff87eaf43ee088d0814660886258982889c2eba816788a9b52d80f0ba02521e" },
                { "ia", "4eaedecc24c6c30231bc94f8dc82cef113d8c562c02a2e69df2de22427f128f894aebb8d60b364e6a7f65bd8388c48cfcae0c56b0cba798bd5853c78091f6ec6" },
                { "id", "ca6bd9e40d13d8a34c9fdb2e44a8566f0ce1bed3f36c0f2be01f634f5209e5cacae21d971a901fef40d2803ae8a20398019c007670063ef1c0e338728700c202" },
                { "is", "dc40771b4fc5c23b52b1db168e52e7bb7f323bad9ac19aa97dcfbce453be11d30738d8299643ae73d4ced4f66c77080c64d7a229fadb6ae9c1066d173083a579" },
                { "it", "0b2b8e871996fa028c760f21731446b3a3336a0dd14f0336c57ed025fdf845b68f619110c410192e747f7a903f55d9518ced9ecfb821348ac7d2189fcc2efd46" },
                { "ja", "7f7c1d2c113f8e1210e1069e7f2ef2b0df898e25d7759feebb252f6e772f79523e51848ed3405944b6f3a275b09740e3c2806830c6c0a9dd1f130801b4efb443" },
                { "ka", "caf80544baea4cd62168e01599b3e198ca8febb87cbc4a3c14523f61f448b5e22746b5e8808712e5241b9708f8cb48e2e6c2922bcd20671c83088b7fa17336f4" },
                { "kab", "106cfeee6c671fee7692d5429c1567998a93eaea3604a7e2081be704f8c94cdc12d2c9d4cbbe1ad5c0551114a0ad732cd595da466097309c8860c1d11cbdfad6" },
                { "kk", "8f381e5933771f2ba580cd37e4742164cadc638c0c3e34efc44d14e8e870cbe18e83c79a3d2a43c3782cb159384e3e199c78a8e75dfdf2e730c03e7377c84949" },
                { "km", "d45158575bc179bd77399a4a552172480db6b1f9b7c830e4ccbadc2ca286e8f54804ccf178a4829c2b4609bed041200d3ebb77db49835abd21305c7a8cc9498b" },
                { "kn", "0ef2c4c0c4ffc1b902fa5a879ba14307763af84cf5b0e25716b488b77a00367706bf953e79bf6ada09e04b4ba04921587a8a4b0f8fb5ed24e3eff724a06d998e" },
                { "ko", "566f61726212722da02ce23c7de8c373caa0066e98a16b8f0c92cfb674cf13d222fbd1073409ad608070b76126e3d8ac4be881859bb55b993850bc583e8f6c97" },
                { "lij", "050484cea995fe55673ad021b054bf7b68eba9269bee56c86f74383ae25d830288d51317f9439abefc78fde1afee0da8a85acb717459d6d8b8bf389189b251c4" },
                { "lt", "d2a9f700d815288dd55a2546307c77f284bfc62e34023cdd302fac2a7c918a9c2df458059d16dca5b44911b38be60c6068caa6a0e0341d28c77dc610d63b0802" },
                { "lv", "c192f80144e6522125f1e16634cfb34bdf8e852ad58503a8cce4300f82d70e2c1bad10f4a37e73cf78dda4e70941cdaf482e8e8d999b5f85f0da509e8440afef" },
                { "mk", "2eb188359aa63aacd3f6aaaebd1c6943b8e5538df861ddd618219fcd17d7a60a2c469cdea79a34703b4e52388841ffcbc024001407cc9d111a782110bfa50acb" },
                { "mr", "7ec7008d2c52e0ff9146262d1b309576b9b64d4a1b14edb409aab7ec03d032ad21fd06f0e8185eb293175cf53c52526dede12e6a3cfdf2d8de95e06bce8c0d3b" },
                { "ms", "29a962bd009290ec24c491db2a66e990a4b9b44ac824b77ac6e846d0a0852f64c8318ff319b7eb55e89c95bd133111cbf6bced15199a02e2fd6cd5054ef513ab" },
                { "my", "7b063ad77e3f9ce772df017e8d4d20ccc9c703a0b8c25f35b89b1557fd48e673b85b10cd90814755ccb3dd5bfdb3342db523a5cb80a0c46d05fa6fc7e1319741" },
                { "nb-NO", "80215df47b18870097f4bccd83e4ab946daa0199dbd15f583e7cb5cad0e45dcbf17670cbf9f28c1705b3d33d3473c495a3e95a1a44d12b33748815059ff63565" },
                { "ne-NP", "c9b51c2b3e7b5309e77f3582c6cac1ec7e1614b8879d59a2910dfaa598fe37fcc870a6a2b75bc2d2bd032a4703a00eacd01a08b8cd8d4bf35633d0af88f054c5" },
                { "nl", "0b89219cde9811514d8accebb1f3aaa76d2e3485d4353a70953e48163d117c771a296de0dae4c899b01636fbedd037bab84b6308472fbfa8c200c5e4cb6566ce" },
                { "nn-NO", "e6bcd7d061cf91b9be53340e5bf02539e1c60a82601dfe0c6d58276f243257b2597e553b5a307941c7c53ffc0143cd3a8511be561fc963f72965aa0c24e75061" },
                { "oc", "c60ab8f0369561dd294451135ba9b7a3f3ab8e523dce545580df82754b8ac973aa6ba05cdf050c85843be1f25f0b2f675bad2fe7e60fe1657c1ac2a88d8a325a" },
                { "pa-IN", "45f26b58ec7bf638cfcd179ba3ebb68d5168d3900b3ef65cd3eb80b0ade5a462ed57e453c80fd3d2899a47e42a849d34c821af204fc3c3c694c9cd807cb4af87" },
                { "pl", "867ec4038d4cb1f4ba6f1ce7f514d51e1635fddea11b9cb32d3e6dad39ecf6085c94133c2ef00f61e864b0cc222eb016732046e43f3e5252778e972f6cb9d576" },
                { "pt-BR", "c56c5dbcfaca490f617fe82b298acf3e36fcebc3ef6427884f755ab849231d606f2fd8e5d29e052033852c062601293c5788e30d41601d7dc39f51bced4b96eb" },
                { "pt-PT", "f9b142e1e898426d67b9382ccb65236e0bb83c29d0a630ae802b25f706dc525a86a3ec30628a5e0a97c8a795c91780db3a59c2893108d61962d9c982e1e03936" },
                { "rm", "3ab440209ba4c84cee48619de111a83ef04f0c8fbbff867a4f94399dd76e6868bf17b0714787a080d1e0c5f123faf88de530bc5e4bd23d7de0f5d53263a29055" },
                { "ro", "05490f9d915653647433b60e001b209ecf1ef339d1ac79c8b0107886c465e500b150fddadb2296740e9fda47ace73c901585676160c2de3942170fa7ac0e0750" },
                { "ru", "00f13da3357d490caff801475144103adc4dbdf20d8a5197b9fc2dfb567439baa7b1ba74ce864c9062f72030ae817af9d509820fd17b4ffb932bc6fae4257e29" },
                { "sc", "7b49d5f16b9ffa28674a74e4a397850e44b63eb77d7c4b6104c832227626f89583e077fa4d8cb6d7085af9f5d7004efba3a68f99c58cee1434d3daddfb8b3aed" },
                { "sco", "872e87743626c4f27c9e3107c1710a31e430f4aa0bac86dbeb02cbced3e72571c07b2fc44045a24fee1d9e8eb825d0f4a3ee13bceb0b87dcb54d2e1fbb2c9f0b" },
                { "si", "ad589cc4f8acfefa60796d11e1bc0a56e6f47bd133e3b395f3b567c02b1633e289ca65991dce224e92605c72e903a0511724077055bbf337a04558b9b43ba8e2" },
                { "sk", "a4d4a53f48b843d2990a5fa9c2786489d84ec18b962598c835e10d5a54143b0276e0f57e2642522e6249d89248c33e979ba2735322d9ad0131f9dfe41fab9163" },
                { "sl", "977521c4c664d83f0d6f69711c92bb930123c13a880a5676d175def139ef887789722dbdabeb9ffce78691b0606a432ee06eafbd76a36a2ce68bcdf32bba4cde" },
                { "son", "0edc0960b9c98de6f42f022f55265c7787c5c5e4d468aedd16ed90c3cb33613be23183422c6067aa621ae9ff473e1380ffe63ead8f9f4c216303bc5a57a5181c" },
                { "sq", "156e39699f945bc8538034ac8a4d3e1b0c0c44f433909a9a4ca5d3857b39ee15f10bf0b326a6081e324d5aa393fb3d6804c561be92750bfe6e598b89cc439e89" },
                { "sr", "30a27e25eb72c8a35a55c72730e3c6258725907b1aa04bb255fc50540cf288de0421b492bb1ca1a9490b6954d4056feaa305bf580dcb8bf563e0e94e0e1aa57d" },
                { "sv-SE", "be38ca0b4a4ebcdd19748a4c27ce647f916ba4d48f78a658a45bbe0946c34e486a0f7abb0f7b18c3e42a6b0b18fdc8ed6c33014f8658dcbf53f93b8288e7d808" },
                { "szl", "fc2211e8c55cfcdba9424d0cecf91171819e985a4f369a2fd6d459fd16536689c2cd109f7029c9214c6b8baa2d385a3b1712771ec1d170880db2ea64964361a6" },
                { "ta", "e8ad2207a1ba7c51878b7b851424dea00bfcef9a8fbc161f2fc18d8c5e40ff087b714ed61a150d4e106139df9ff34e5cb1dac40372a2979f7f5724f396c8cfad" },
                { "te", "2ed42c09ea8a73186d2bab726e28fb3a07e77ca100446a0aed8454a35c8dbcf7d11504e4e9f546a214041b0af4322ae316d05171426440a5167ef9f71bec7266" },
                { "tg", "c7bd27b5ba2b45c15e390242967d5e2c6c558602ceb04d07a9702e0b0ca68a30072a959f1ac2c01d55adf1e63a925dc00993600942520b55a2f82525b413a15a" },
                { "th", "92b5b60613227d024d782700da1d15bdbd47631c39431abcc5e769eab78cbc06154bfc8f9e290de45ec1a84aeea32bf90294e33d8cf7f2d618c62d0e74d4830f" },
                { "tl", "9fd1bf69b4fe22bd0657f52ce2d31ff8280a1c8dc837656bb68f5f7d3f58cd08a04dc5f1225ebf251b576e45378c238e2bafbc7b02ea025389863606ea2d006e" },
                { "tr", "5661b3c7907c6b6a5dd8aadd2a0ac07399a8446c400b041d6e96b25c71109b09171fd3532397bf15658bddfe4745ccc1cabdffd327ecc6de1ad9891ab12aea6d" },
                { "trs", "eb4e1cb4b5c817b8aa4600036f57022a98f251d0fac0075a98b54b55f242e406ed7474123feeee7565bc518a926aef1bbf14898490dbbfccd7641f1e6648f5f8" },
                { "uk", "87f140d6c9cd3862677c42bc7cbde4fe99bc0d866985407b7973044c14c1aea0e2a86b77493f663b1c1f656f328f92b4bd48799371f613e7dbd7cbca812b8ae0" },
                { "ur", "731aa58ffe00d5d95327883850adbd008e4a03af2a6ffbc07aa8b97b54f94a4d5ba569be1573fd80195af20d8fd26666d91901931dc6f845e42f57681976693b" },
                { "uz", "87407c58f278f845300a6a35d86207776ac744ff9bee7b26dba3de86bdfb929407d3c592a2c7bd22f2f70b0f093c5d2ddb8dd76b652ca33a8b8ba6dbc9106437" },
                { "vi", "fba9d9fae96b04e8b0cc1a94cbd548fe1792563c2e1da4b2ef9c3e3cd632d347c3a1c88cdb9e33d24d234e11c2fc42499aec5ad1fa86bc2b4943d1829140ae07" },
                { "xh", "2cfb8c91dc0e22af9dffef60bb08002bf8fa21b869c567b406ea24ca98ed09aaecd83a328ae6aaee7dc1c148db2d2d74e99169364d333079b44c58da480c8b8c" },
                { "zh-CN", "755411b8afea41f314b7852f578953466d741da2412c07988e2ec429d11238e164afb9102e361c62316c43c14b2ceb699eced288e32116c01494bd44b7677e02" },
                { "zh-TW", "a9e2c95a66d1686811299d4aeb034acc84323dd784440f6013a80610e0d26877449a2734e693ba2f02f003a5b5a8d77bcd3eec856e26d2b7feb8088776a8be56" }
            };
        }


        /// <summary>
        /// Gets an enumerable collection of valid language codes.
        /// </summary>
        /// <returns>Returns an enumerable collection of valid language codes.</returns>
        public static IEnumerable<string> validLanguageCodes()
        {
            var d = knownChecksums32Bit();
            return d.Keys;
        }


        /// <summary>
        /// Gets the currently known information about the software.
        /// </summary>
        /// <returns>Returns an AvailableSoftware instance with the known
        /// details about the software.</returns>
        public override AvailableSoftware knownInfo()
        {
            const string knownVersion = "117.0.1";
            var signature = new Signature(publisherX509, certificateExpiration);
            return new AvailableSoftware("Mozilla Firefox (" + languageCode + ")",
                knownVersion,
                "^Mozilla Firefox ([0-9]+\\.[0-9](\\.[0-9])? )?\\(x86 " + Regex.Escape(languageCode) + "\\)$",
                "^Mozilla Firefox ([0-9]+\\.[0-9](\\.[0-9])? )?\\(x64 " + Regex.Escape(languageCode) + "\\)$",
                // 32 bit installer
                new InstallInfoExe(
                    "https://ftp.mozilla.org/pub/firefox/releases/" + knownVersion + "/win32/" + languageCode + "/Firefox%20Setup%20" + knownVersion + ".exe",
                    HashAlgorithm.SHA512,
                    checksum32Bit,
                    signature,
                    "-ms -ma"),
                // 64 bit installer
                new InstallInfoExe(
                    "https://ftp.mozilla.org/pub/firefox/releases/" + knownVersion + "/win64/" + languageCode + "/Firefox%20Setup%20" + knownVersion + ".exe",
                    HashAlgorithm.SHA512,
                    checksum64Bit,
                    signature,
                    "-ms -ma")
                    );
        }


        /// <summary>
        /// Gets a list of IDs to identify the software.
        /// </summary>
        /// <returns>Returns a non-empty array of IDs, where at least one entry is unique to the software.</returns>
        public override string[] id()
        {
            return new string[] { "firefox", "firefox-" + languageCode.ToLower() };
        }


        /// <summary>
        /// Tries to find the newest version number of Firefox.
        /// </summary>
        /// <returns>Returns a string containing the newest version number on success.
        /// Returns null, if an error occurred.</returns>
        public string determineNewestVersion()
        {
            string url = "https://download.mozilla.org/?product=firefox-latest&os=win&lang=" + languageCode;
            var handler = new HttpClientHandler()
            {
                AllowAutoRedirect = false
            };
            var client = new HttpClient(handler)
            {
                Timeout = TimeSpan.FromSeconds(30)
            };
            try
            {
                var task = client.SendAsync(new HttpRequestMessage(HttpMethod.Head, url));
                task.Wait();
                var response = task.Result;
                if (response.StatusCode != HttpStatusCode.Found)
                    return null;
                string newLocation = response.Headers.Location?.ToString();
                response = null;
                client = null;
                var reVersion = new Regex("[0-9]{2,3}\\.[0-9](\\.[0-9])?");
                Match matchVersion = reVersion.Match(newLocation);
                if (!matchVersion.Success)
                    return null;
                string currentVersion = matchVersion.Value;

                return currentVersion;
            }
            catch (Exception ex)
            {
                logger.Warn("Error while looking for newer Firefox version: " + ex.Message);
                return null;
            }
        }


        /// <summary>
        /// Tries to get the checksums of the newer version.
        /// </summary>
        /// <returns>Returns a string array containing the checksums for 32 bit and 64 bit (in that order), if successful.
        /// Returns null, if an error occurred.</returns>
        private string[] determineNewestChecksums(string newerVersion)
        {
            if (string.IsNullOrWhiteSpace(newerVersion))
                return null;
            /* Checksums are found in a file like
             * https://ftp.mozilla.org/pub/firefox/releases/51.0.1/SHA512SUMS
             * Common lines look like
             * "02324d3a...9e53  win64/en-GB/Firefox Setup 51.0.1.exe"
             */

            string url = "https://ftp.mozilla.org/pub/firefox/releases/" + newerVersion + "/SHA512SUMS";
            string sha512SumsContent;
            var client = HttpClientProvider.Provide();
            try
            {
                var task = client.GetStringAsync(url);
                task.Wait();
                sha512SumsContent = task.Result;
            }
            catch (Exception ex)
            {
                logger.Warn("Exception occurred while checking for newer version of Firefox: " + ex.Message);
                return null;
            }

            // look for line with the correct language code and version for 32 bit
            var reChecksum32Bit = new Regex("[0-9a-f]{128}  win32/" + languageCode.Replace("-", "\\-")
                + "/Firefox Setup " + Regex.Escape(newerVersion) + "\\.exe");
            Match matchChecksum32Bit = reChecksum32Bit.Match(sha512SumsContent);
            if (!matchChecksum32Bit.Success)
                return null;
            // look for line with the correct language code and version for 64 bit
            var reChecksum64Bit = new Regex("[0-9a-f]{128}  win64/" + languageCode.Replace("-", "\\-")
                + "/Firefox Setup " + Regex.Escape(newerVersion) + "\\.exe");
            Match matchChecksum64Bit = reChecksum64Bit.Match(sha512SumsContent);
            if (!matchChecksum64Bit.Success)
                return null;
            // checksum is the first 128 characters of the match
            return new string[] { matchChecksum32Bit.Value[..128], matchChecksum64Bit.Value[..128] };
        }


        /// <summary>
        /// Determines whether or not the method searchForNewer() is implemented.
        /// </summary>
        /// <returns>Returns true, if searchForNewer() is implemented for that
        /// class. Returns false, if not. Calling searchForNewer() may throw an
        /// exception in the later case.</returns>
        public override bool implementsSearchForNewer()
        {
            return true;
        }


        /// <summary>
        /// Looks for newer versions of the software than the currently known version.
        /// </summary>
        /// <returns>Returns an AvailableSoftware instance with the information
        /// that was retrieved from the net.</returns>
        public override AvailableSoftware searchForNewer()
        {
            logger.Info("Searcing for newer version of Firefox...");
            string newerVersion = determineNewestVersion();
            if (string.IsNullOrWhiteSpace(newerVersion))
                return null;
            // If versions match, we can return the current information.
            var currentInfo = knownInfo();
            if (newerVersion == currentInfo.newestVersion)
                // fallback to known information
                return currentInfo;
            string[] newerChecksums = determineNewestChecksums(newerVersion);
            if ((null == newerChecksums) || (newerChecksums.Length != 2)
                || string.IsNullOrWhiteSpace(newerChecksums[0])
                || string.IsNullOrWhiteSpace(newerChecksums[1]))
                // failure occurred
                return null;
            // replace all stuff
            string oldVersion = currentInfo.newestVersion;
            currentInfo.newestVersion = newerVersion;
            currentInfo.install32Bit.downloadUrl = currentInfo.install32Bit.downloadUrl.Replace(oldVersion, newerVersion);
            currentInfo.install32Bit.checksum = newerChecksums[0];
            currentInfo.install64Bit.downloadUrl = currentInfo.install64Bit.downloadUrl.Replace(oldVersion, newerVersion);
            currentInfo.install64Bit.checksum = newerChecksums[1];
            return currentInfo;
        }


        /// <summary>
        /// Lists names of processes that might block an update, e.g. because
        /// the application cannot be updated while it is running.
        /// </summary>
        /// <param name="detected">currently installed / detected software version</param>
        /// <returns>Returns a list of process names that block the upgrade.</returns>
        public override List<string> blockerProcesses(DetectedSoftware detected)
        {
            return new List<string>();
        }


        /// <summary>
        /// language code for the Firefox ESR version
        /// </summary>
        private readonly string languageCode;


        /// <summary>
        /// checksum for the 32 bit installer
        /// </summary>
        private readonly string checksum32Bit;


        /// <summary>
        /// checksum for the 64 bit installer
        /// </summary>
        private readonly string checksum64Bit;
    } // class
} // namespace
