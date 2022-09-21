﻿/*
    This file is part of the updater command line interface.
    Copyright (C) 2017, 2018, 2019, 2020, 2021, 2022  Dirk Stolle

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
using System.Diagnostics;
using System.IO;
using System.Net;
using System.Text.RegularExpressions;
using updater.data;

namespace updater.software
{
    /// <summary>
    /// Manages updates for Thunderbird.
    /// </summary>
    public class Thunderbird : AbstractSoftware
    {
        /// <summary>
        /// NLog.Logger for Thunderbird class
        /// </summary>
        private static readonly NLog.Logger logger = NLog.LogManager.GetLogger(typeof(Thunderbird).FullName);

        
        /// <summary>
        /// publisher of the signed binaries
        /// </summary>
        private const string publisherX509 = "CN=Mozilla Corporation, OU=Firefox Engineering Operations, O=Mozilla Corporation, L=Mountain View, S=California, C=US";


        /// <summary>
        /// certificate expiration date
        /// </summary>
        private static readonly DateTime certificateExpiration = new DateTime(2024, 6, 20, 0, 0, 0, DateTimeKind.Utc);


        /// <summary>
        /// constructor with language code
        /// </summary>
        /// <param name="langCode">the language code for the Thunderbird software,
        /// e.g. "de" for German,  "en-GB" for British English, "fr" for French, etc.</param>
        /// <param name="autoGetNewer">whether to automatically get
        /// newer information about the software when calling the info() method</param>
        public Thunderbird(string langCode, bool autoGetNewer)
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
            if (!d32.ContainsKey(languageCode) || !d64.ContainsKey(languageCode))
            {
                logger.Error("The string '" + langCode + "' does not represent a valid language code!");
                throw new ArgumentOutOfRangeException(nameof(langCode), "The string '" + langCode + "' does not represent a valid language code!");
            }
            checksum32Bit = d32[languageCode];
            checksum64Bit = d64[languageCode];
        }


        /// <summary>
        /// Gets a dictionary with the known checksums for the 32 bit installers (key: language, value: checksum).
        /// </summary>
        /// <returns>Returns a dictionary where keys are the language codes and values are the associated checksums.</returns>
        private static Dictionary<string, string> knownChecksums32Bit()
        {
            // These are the checksums for Windows 32 bit installers from
            // https://ftp.mozilla.org/pub/thunderbird/releases/102.3.0/SHA512SUMS
            return new Dictionary<string, string>(66)
            {
                { "af", "568db4fe7cf6464972fc7d69c058c290151c1f28aa71f24a2fdde79d7aa9935ec72a78cb76091c1527d074064b415e7ec6ab15fb62847578176a27a16b930ad1" },
                { "ar", "6e7a7411c7f1664f5d43e99a2f891015b866b6cbc0d646726973bd0f0ee1c42dcb111d405ed6d9291624ebf472195d09b3470248159c6866f70a0b96ca9101ad" },
                { "ast", "3b10fa1c7dc797e8744d2a9e827072420abf1af3e6dcab56238198a3e54a8af80ce242e64893c4248b035f131e798d212a2f1b1288bad4e5635081e6e8cbb8dc" },
                { "be", "7146e38b6555358e560e2595e8383604be243b187416fdbb223dd25de1c5a46cb6c02dea2623223a3383a9dce4af536af292dd1bcddbb3334e534a23ab3f730b" },
                { "bg", "b718ecca1983e652e7604ac6c90ac55a46b80e79ba4973cedfa3d7573ac7dbcef3edea235e9aafc12a2bf6ccc9b97fdb309041be8328c735df70697a9f84d5d9" },
                { "br", "10908c58c68104bd107a51571929b2d00fe30e0885c02682bf81569a11d87e69e3007c7c732306c4854215cc7670582c8caa65bf03d0476997ed51f5b9dad037" },
                { "ca", "ffec66113caabcb93c2485279e70d0eda6479d51855f77837109fa08f2053ff6a8601d89c442a10a5310eaed418c6feb4ee78e1e0f689e4bf168d2d3c98cb82d" },
                { "cak", "68959699f50b91490e33eaa8e7be54fe43a14dd2189e4d741aca1e7a3ef026659a038eccca5be3914505057465fc35c8d9e40c1f820d88b4808abe5a329e30fb" },
                { "cs", "ca0d6a4b518f88afa789c8969303dc9795e5481f74d6c0f10bcf1f5908b77f6b050a00be39b5f670337b42141a84a6cab7e4d62da7c5707d9bd4741e39f56866" },
                { "cy", "66109a9dec66bb4f377681dccf6b32e8a5d4a136d2dbe02d356bd8971c5df864935756707a46346e379dd688646469103e773bee041568ac6e1c524291a4ae18" },
                { "da", "9cd0ec090e7d7dbf04da657d8190fb7c7d353eba8d6e755e2b133e7538f5bdf74c54c442c2ea78b99c5e419c36395690ef2ec7f2de51f3992440444b9c89f7ac" },
                { "de", "286bd5ff8abf2b46af4b8f34d1fbb6745541ce86b816fa7f01572165637ffc78bed68a49e142a09420e4719934bcb6653505e4503b2f4cce5db9163a2db8ff22" },
                { "dsb", "d42fad8294a0ba65498eb86fc4c623daf757fb084da5eb9dc9cf3fe620cade58c278392a2f8b4bf658cb8d85a38a99bd12f93084488b1ae6d3dac67e31359daf" },
                { "el", "70a09438086cc569a6cee1c57e980d5a1d433329d1a7d3c2203669b6bfdc4a9cdc3738923db06320d3c5bac9174fa7212e34a840f5406679b29db8cc1dde7527" },
                { "en-CA", "25504ad4dcc371f9415f6832d6939b49b535c270e55fa5166e4bc098ba14571fbb95d80cfba751dba3626831fed281cb8e9a3797b0493a68770ccf536fee07f6" },
                { "en-GB", "043c666c8f520ea2bd2f29de12569af8419e0cd7469dcd297ba97e523e238c633accb5dbaa36d81ec80aa7676910ef7e7791f18ed2e5b19688f7c368226cb80e" },
                { "en-US", "cb42d5e3c8641a6e85d83356de85c8221ce263f7e08991e82765b87353edd8d2a2421f1fb3a09b5f06787a0c04ca71931b36173a6df399e08bf6e3ecf877d63d" },
                { "es-AR", "3e113e90150c86a3d4f2997766555f3a580fc74c7f21907ebf673571ef468f9aaac4fb22ff350bffbac9cb4d56d597c51d0c00d9ca8461fb2ac1700c9be1b06a" },
                { "es-ES", "15292bd5ff2ab60537518efa147acda3d9540865a63a01928070bbb327453dc980461589f9f1cbca7e1c3b24131198df7561be11f8f20ac3529eddf9547f0135" },
                { "es-MX", "1656983482eb7a34ec513cfe81305549c78566e76e57591a3a68d90b4b47e3d0b980590dfa79a45dcf3d462a8d9dbfb468e9ebe1ef2234dd771fd6ae49d6332e" },
                { "et", "5a3624e099669d8d2b3f951c4ee4ede3dbbf7841b1de63c219185f4aa3c319aa9e2e1cc3973ffa8541585e298896462c7e7428ba2993cc13f5b3e8cf8b86aaac" },
                { "eu", "a0e32f9cacc5bf7dfd7596c3feecc8fd926f3064de30a499343c71eeb3b931f134190590ee45e212767173b3e084287ef471a3a91451928677b94273541da1d9" },
                { "fi", "b89ce5503993b19caf5efe36d800428c5c807d5d001c75fc17caed77ba1c0f9ccdf5020b51ff4ad3eb06be5e730817ad35c6dc71d0d43bb29255c8e8d96bbb22" },
                { "fr", "afc817059683cd0a2ad7120b3848d1fd53052cb348bd1ccb6facea65394ba85d531177aacde58f62f5a7ac14e91dde45e59b25cf930f0f6f5298ff9e019e93ab" },
                { "fy-NL", "7b6f3415c9bfc1b0bceced23a03bb230e759fb727fca9789ce65fc16e019131c151c86bdafc49190ac70c8ec58ff64e88341e9e26dfbe9d8928a540f3f8fcbb2" },
                { "ga-IE", "114691545ee306150446f72d4c69baa3221567ed9905f50abdb74b41d0837509e2365d73b6a6fc1045ec15b80a1536d2055ab5668571172d6512e3c618b41ace" },
                { "gd", "4cd5e7a4052b746f3bfc3ee1eee7f34bf63458177c5928d713da96623431fd01bec9bec9a682053822b88b5c9876803d618c5d7be244b6326cba8602394894ef" },
                { "gl", "e30a067ff029eef6b8d42dfa69f9095ce0a3e2867f7660f1ef914bd9e206289321e987212103274010b166eabb31430987818408514af6060f271074317c804d" },
                { "he", "77ad9aae4a4e19f2b255c9e36e612c6439d2f8e881279b7e72bffc50ed2709bece5e86d55a36ecd1e90fdbc534714e8e977577d3ab1b71679218978d8d64c3fc" },
                { "hr", "60b726a74cab992161ffcd21388120c0095f3aebf765f4cfcb7ff5d7f1fb2e90e44ae343e1911722ae4c33608067554921b5bd6275e4eaf351f1f71a7ed74bc8" },
                { "hsb", "11b513cb61f5fc1b6d6ddaac63a883170da3771dd663f3fc718b28ec87c1b0756d983c6670e10af82bea0b65708b8e090c39887db292258f8653594652760e78" },
                { "hu", "3c143ebb3620620995396b47961ea9d00752d950100292579272ad5230b2297953ad9bcc8675c51258a8ea179d6d06b1f04407f25a045230a316fedf74e3da30" },
                { "hy-AM", "17b2b8fac5d6b93446a8c9f4e5cdb983d3f659df52a18417d565c8c342a6e065c7364429ede1a3ece35107c64f4c8c8e757c57b40318f19a60d91b7408566b92" },
                { "id", "3f5fa33e30268fda524decdf867590f84eed30994dabef251a44737e78feac85f9c16c6b04ff21828804b5c22d82dfff2cd9bdbb37b4b0da47bfb49b30c4e680" },
                { "is", "f90d349eb049772f805d5188ce29ba3b00021705c05c8285ed407a34a014381fa29636e00ec4d92f7ceeb8682fa9c99c37bf2e090572ea0e3ff844ff193342d0" },
                { "it", "971b4b6b2b2ba580a6857eef0c855c2d5314353cc7f6f131753400ffb1d168978d14f3b4cad3a9e33eeb5806a13d357e5638f66f599ed26dce0ef20383b6d8c0" },
                { "ja", "3cafa6cf897433eb21a2a3736fbaba6c5c88d90b1a1b445181e94137bc777ac9b6d7e71e5975e06ec266c67d3314bb93532fc815315274f4d2b22c530bfab4a8" },
                { "ka", "eac03b5f61051cbd23a483093c5221c043b4d7094c7617cd4d134883fdc61d9e1b50de3d2040c336cffa4881a921248e4a87e0443551317203bc99dc7189df11" },
                { "kab", "b43cc5026aa93f8318b5917d35b2b6876de73b97b47cb3b5c611b79b9d28e92f4c3c8d2903de74d002becd2e554c4b2885200d306179bb8f9db9eeaca6cfd512" },
                { "kk", "7be19a9b61c5160da022ee5c1196ee8aa406f41e57c7507d8368ca324142db8f3b2ec7225b96a8ea77b1d7bf8bf9d2e988bbe50d5aaa0f870ff081dc88526fcb" },
                { "ko", "2fa64ac030c64588e9913860503fbe781516ef22d7d2f03ca9685ac7527c36c2f6dfb46a5da8711f40b34a4afd3e93441570be9d967031de960cc9f25d2587a0" },
                { "lt", "b8fefc46adb87951b5e73256df33a785de3ef8f6638e5777d4bbeef4eeaf8429732328fe339369348b87adedc62215d712ab2a7e0f0b673bde3528318fde5a3a" },
                { "lv", "3dad61da123e9fe48de1ba11201602820d349ee51b51997a5bfbcd2ccb69fed03b959f86b6d1b9c3646fc20a03dc5dab9ec6da2b8b55d0f46f80031ed6b81553" },
                { "ms", "5a35f747a5838d386f28b5fec3846e174fa7a427dc1b84c9a835c3e1f07b9067614846c72657fda898b33e0745a73c0a949ba531d11d03f523dfa0098bf45bb1" },
                { "nb-NO", "1765c253752e07b17a9f4c6833467b15ae5d33e310e34190c0753eed17212c438e554f31c6b35c064b6ca53217bf7c389bdbe5987385af937e014f2b94a27b7f" },
                { "nl", "36f5273cbafd704ebdfd19f0f45e7ac79dd9ca2b3e75ed5b14b72ad7b70191fcbbf69752d41ddb12c0f8d5e28bdb0ab6515a38c3269c8b497978576fa06ecaac" },
                { "nn-NO", "29f36473d3d77467f07761769f27a05041ead7d56afc54406b56b1d075c1ee15bface3c05b320b708ca1cdd0048475b45f48a0b183dd68c1a9efeb7a12a0faaf" },
                { "pa-IN", "81474b2aafb1288961dd76d186cc1b8ffb03e001b846ddbccd9af3a9d2aa8b5fe3a6cb5fe7f0c4ad777b041035910114ed8d444ad44beb66d8399a90f5720ecb" },
                { "pl", "11dceb4b96e7ca6e8186134f2668e16d20df822226d17f915fcd660cab14447f876f71c30a1c31efdc9024a08746c2d5a85944e4778cccd3c49af750ed6805b7" },
                { "pt-BR", "cabf3dea50e44dc8e82e79a623b054b016448a0cfbcd083d726da8d575ccfd2b2344656c933481a2ab0bb472b6ed6d80f66cefdf21d77ac6b59f80937aa381f3" },
                { "pt-PT", "4223cd61384ddbe093ba6d199cbff69e9ceda5af2cfe66d1abae53abca772964b9eeda15829e09e9213adea4b7ad680db16ff3c04ecb26a5f2cabaabfe45a14d" },
                { "rm", "63fa5ec749d6d8cac66041023d44847edc6554d12d288d5f2d37c9cc5b5af5b1ae828a4f3b7ec153986594fe1daa280ffa5533dc529bd25a2bdedefd49811bf9" },
                { "ro", "ca6a3ae8003c4614426b55affb5c283f91b877afd08df0597aea7540e0e7126a0a6a69cf3150fc33763bea77d435b81c0004535db924498e4cdf1cf6c9344301" },
                { "ru", "9159810ae5102f0b5a1a700443e36026da80f1b0f19885004cc7daf56a413cf3aa039d74e599a09d6b520c0edbfe54e75265996c3d17eb28bce08579fa017f8d" },
                { "sk", "f4a55ae3c72b17fed0ac581a8cc782aa6a54ce052b9b7b0e53fa684ce56116e63d55ef58b7637dc2148277858721ce395ac3e338c228f1911a8ab0e22a818ab5" },
                { "sl", "4d617c316569c730ab78a9fac1972b92e2bc06859d7b9fdde75f69ad66cfdfb3973e9539c1dfc9ccaa59f1187963059792e82ed7c2b2d21e2e28dff843645adb" },
                { "sq", "53d11f527e0031a3f87bf05112ec1c804c508a10a06c6e0dbeba4b8f174f77e5fea786e2fba8a870be72e24b70436628cc4e3cc05b48a4d55f9880d4517fca6a" },
                { "sr", "f69fc06d1cad229297f27bcc831b2ee2b7b4b3d469f87f9022e8a3f0b09d6249bbf9c818632102bd8e91b213d4c2b298a3521062ec4d934c9b468b46224d989c" },
                { "sv-SE", "f255c5122fe657bf5215b3e7af366d807f8ecbf3984941ece65ec2cc56a9a3a9c80966d22f665e63513c3053964f742a2345befe28a900c1ef8476c2ca16bfcb" },
                { "th", "5872d79a2e2fb92b08d7a9439f422f377e3286c0e3ed593a9eed5532e4c7b5dad7d46fd41c2fa49fa9a2309ed86c372b86abf06c9ec7eec27be224c48fca1f2f" },
                { "tr", "fe84f10513caf1b20a2173bad33c34f30e1cff126f2b152dc353fb1e8442f3cf628145afde8d5302306fa970ee91dd117bdd7c909ece2220fa800d21277766bd" },
                { "uk", "0662dc932f20e51f515c11e928a689eac28daf768b46a92a49ca625832c01c8eda57d651880fc328c48c6de6014a1c96a55bc8fe543c553df581bbb1f56a36f8" },
                { "uz", "c611c303787677fb4d08452c4f3f0671f1e1e99a29511560e6a941c86169aeae813ca3b2be11ff4ce93baf12efbe6ba156e64ba911b7cc0aa308e5804ca486c1" },
                { "vi", "32a17c4a73697b1e3bfe215846a3c72615c72a01fbc658741957cf54f6f0bd929620633aed3954ea005d2a621c3f6890f90b953af7b6f3a9d558e68e4dd91e30" },
                { "zh-CN", "dbd39ae9a7690ca2389d997dff6b729e61665196d4e30dd6dc235c44903ead20ed3e71954b59149cd9e67d11ec5cec27840f9ad32f5470d0f511167a074dc2df" },
                { "zh-TW", "5ad67977dc146604bfd526c090085246d30971faf377cb58b6c250d3d9b883590f6d6479f4fbc2c0f2bf83607b86318e5895fc21f3b89671ce3b446810a382f9" }
            };
        }


        /// <summary>
        /// Gets a dictionary with the known checksums for the 64 bit installers (key: language, value: checksum).
        /// </summary>
        /// <returns>Returns a dictionary where keys are the language codes and values are the associated checksums.</returns>
        private static Dictionary<string, string> knownChecksums64Bit()
        {
            // These are the checksums for Windows 64 bit installers from
            // https://ftp.mozilla.org/pub/thunderbird/releases/102.3.0/SHA512SUMS
            return new Dictionary<string, string>(66)
            {
                { "af", "dcf5af2915d31db6dd22e40aeaa65fb2be44692a518c4a61c36360b8163d575cebe4b20bfe4ae8b2ea7186c0c3a5e2b454be9707894ebff0b6419c21bf26870e" },
                { "ar", "73380c964f4486f71788e3eecb8b747d5aece56e82fdac835f476ffd76986767dbf2d8c507e4ba0ffc79dce18a7509ce70104c5da44e324b3c4752bf4cbb5e00" },
                { "ast", "687dd3933a3d51e6ccb031564111584819094f9197ffe43f9fe7a884618446fb97016c800ad2c4206cf0bf0ad2d47b3b983d7c82a861839860a5093bb34570ac" },
                { "be", "231b635c28d63da7ec360cb33ee2ca2ce830c3355b5cb0b22143b6f898c18afef1238794af43301fae668cbff53f878c7a614addb0b46ed5d0fe21332b1d7d5e" },
                { "bg", "d60ecefdc18caa7b8e5f579851ce847b3316f13896dba94ca957c7de1563f5b7d456ef213282026b4f3a991cdcb2200d8343bb362ccf2a95fa50ba7bf83ee735" },
                { "br", "5ffe3a70267c930428908d388fc54769c8d89ddd0ac3d005a45cbbe7537678b095abebc0d5e30ee0286047d0981adbb30882db71a958e1ca8912810500bd6885" },
                { "ca", "0a0bff91eb46629e10bcabac4c50c67cda6a76aaefb1a90c6eeef388dbc5f0e0b247ad428ea836df237c497c97f56285f506ec73e69706d48bc6d82108fe2203" },
                { "cak", "f2d91a83306d4421789e26a26aceee5b183503130ada072f7fa864bad07a6c7eb9baa9d90218b8adf5839480ad1cf8ca8aac01c328b019911903d985b8182b4e" },
                { "cs", "6972fa039ddf1802b528238cf82cd431f52ec7cea47afeffbe0bbc12772940ebd6b3484ce27804761e99e5e9affe8a57d28c7cded9eba86680260b95d88fbe55" },
                { "cy", "267560f73ef2e020551539ee054196160f1fa0c657e57d15c2a4158349396addc3459fd0d641c643f94021000733ff3943a7d7920888776f507c95b0f9a462ba" },
                { "da", "6b734e49cb16a7ba492f9569686596d3246c6de148e01aa6fa15d7e56a6f9dc0ffab5ebba2ce2f908af07da000965ca52fa780ed9a04099b35509eeaabb9abea" },
                { "de", "cc3f616ede7d4d33b1027afe29d30301aa389e44965cc5ace750a45e5d6a6eb4d81a19f8b7527a442d198e860fd75679b2a6f7a65bfb8157b94f6890f54fe2f7" },
                { "dsb", "b1fef9752a5e507790988acce2771e68fa097b3f0e4b982d776106aa262d8d9861a117d7b21e30239b1dc606a6e6235908df841fcd7bcd9a0d0fafa4b6295728" },
                { "el", "0cb151f1dec2478d893a1b41fdc70d9c4b0553babec39e618ceebc2ca7625a00e5e81230040b48f2198232fda9a7918579bcf170b9058d0cd8e7db98d6a8b100" },
                { "en-CA", "2577c5c9d07829850064819af9c13a7b8f72f955b6e0ef8f662fa3eb22869c88225610fbcc845c21426827653fa64d205d3e67309b0da042fd7a35fc93178519" },
                { "en-GB", "a12f530ea7f1cc7e8134c20113e22c12dfb5cc96c10aa11feb3be2b46ca8b40d875dac63d7a1f04c80df18bdaa37d382b6110100e51cd85a6320ad39c8d4af7f" },
                { "en-US", "1b650b400aa985642e9489c5756cdf54f4f136953b12c98800562500e002cd7cb998fd82058b307a4ac0b17a882fd593682e2221297e2292235e0b5042318fc9" },
                { "es-AR", "5064fd318820ad109f3cc8c453f26ebfac7f481c02fbd3c8b8436d74a5d60f86c80c27ba1849330527e8d20ff2f948d008278eebb6af4c290d2d2110794a54bb" },
                { "es-ES", "25e232c83db80f02ad2602f302095501b1b78bda926614c33715faf52158358bfc0d8f2ed278ab55efd26339bb7746a196be3c95cd02ed9737ed7ce2ae13adb9" },
                { "es-MX", "86d0a208abf1b3b5915bbb0dcd028ea04e3c09170de2a09d808db4811b8c924bd10b1938f33933a7815089f76e7939e1eddb22977a0bdafc0076438c4b25d275" },
                { "et", "764e8f6090edcacf2e1a26238638fa79113b0323de342a74066417128dce4b561a9212d7c1108609969e156c956a2cb39fe516793badd858a1edf3d4edaaf81d" },
                { "eu", "b2e65842c490a0e025d06c442bb07e57b86fe4ad03b6f76d2faee9973adb97651745d6b6bee0846310b04dbe915f49f2ee5f38edd17ce67f15b66f42ab55551b" },
                { "fi", "37dcfde7573d3874df9510e8d649f1bfa0fde6c24d330f83a489239373f40e1db4ce475b2d35c9bfc8b1025a4820495e49a05341da853d0b1ac7ca2aba4e91a4" },
                { "fr", "d4d3f30e140c4dca2990692c18396e9d4bff96646710f3dd5caa637b02063378e4423064085b05577dd71d1d1ade9a34427d1392136ae4bba3c0009fd5426443" },
                { "fy-NL", "0553b32608e3f2b8ae0022bc5b4d4f79d246261a0058f27144c8709193dd2bf2aa94fd3eb0255d4c42ffc445e8ff42f34f31156ffa16f8af060b251808b6347d" },
                { "ga-IE", "972790cdbf879bad9303563c03b6909318d8098e31540451612d778fd9635073202ed12b5a6c2c7b03dabed01756c7ba4f2d87a9d8f7c8055006c0e5526f3052" },
                { "gd", "fcaf7737a18c325f9a72b6379012944cf5d5bc61051abbf804a65d867328bcce3c713c6fd93a5874143cc6b5087054797f43bfdadf6cd2e89b8f6f7c29c665d3" },
                { "gl", "6c12ac2c8c98bf90f80af86d5efa52b3039c24217c926a4bfdd0003b8f61126a18f209d6d3f2825b3155d6b0c94607acf1460de7f7d4dfefb54d6ff40c8fdd14" },
                { "he", "6d53807cdcabeb326f77947ee79f413b67b215a8f9954e91ab482d27fa4fcef5d0d9803405401956e1613a24a62cf643e8499921c5febb9103ef3c6e5679d4f9" },
                { "hr", "09115e946455c7b7e01d5a9fe79b09b9ab0eb0425a5431780958246bee9d37fb64e261f97c8f1d1be0d3153c4c5e6615d40578e9c24fa0421ba818ffb99fa055" },
                { "hsb", "d4476390fcd067d971022c9d1eb726df84797d39cb489987040d5845e170dc1939fb2f07f9e3e0afb31fc14bd0912f14c59001128e3826e86624b056a5e1b2df" },
                { "hu", "7dece32ac0cdebd5b54d3e578b79edb4e0272f132e0353ec2aa65030992df0d2cca9a7de32b13b4745fbc88db7fa3619d98adcd0bc3a73a07c76fe976698e9bf" },
                { "hy-AM", "e25da23947b24891f10e1235f5def61943d423e369a169ab7628b5c1457cd477665a9f845b01cc34e46b7ef5bfb89fcbdb202367dcb8d14516863ffe3ca209be" },
                { "id", "dcd4cafd38c5398154b252279c581270a7e6384106df93e09885ff9b1e6e1402fc0256710df2adb5e49a9c447a2b7ed1f625442288663581a5c2d166258586db" },
                { "is", "27b055d38c83a090cc5fb63ba0533c00c285975f3cf49a743e13424149ca6eeb90170769cf73745b140869be9b7b055c7c43fc600a9e559d8120b318eee0c894" },
                { "it", "356d2386e614dc62cefb77dd577e361bd4a36ccf0c468d2d6e638c72f4f02aa81e8ccedb8b1c494460ea643a9bb872174740563478e375e1f3d64dd8ef6a7271" },
                { "ja", "d41f63c91659d03725fc492c0f774bdde886cd1673cd82742aa8aed1746cbec2e86f0dd1f5a381324c3c86c20c2bcbe2a248668e808ee65a73b2bd1245297194" },
                { "ka", "62aeebebf839b15b24ab205bdcbaafefdcd82e57b55f4c46580079e3794792c1873bbf20db71579c0734dfc1a7d356bc0f438289b0a94eab12626e837a64821a" },
                { "kab", "e776c7d48c9870ceb6257805b5839fa3596267c6ec241ffdf6be73fd2c0c581ff5b3a78c021bd1dfc50916d16a898ddaf0d4a4ae55b4fac01ea094c61df21f14" },
                { "kk", "68c03fbe31df741742ae786263ce915e3ed88675c215cc10f0dae0f15716040c318e0d36f72ed963a19060dc8cf8b6f01e87c08c6589e7eb6db626a08c665139" },
                { "ko", "147abaebaaa11db780cbbb0ba37e7f1ac2362563e3aee003b3db5deb5920326a4dc06b0f6ce790d39b2f56ec05c1ae44bec1916932611ab7122918c5e5beca73" },
                { "lt", "61b4e2875362760c3e8a68c0b4ed4803515612b2d5c23d029aab39b05936e7636bb80f97c762c9a08fdad98bbcba78a79ef37480863669f6416c84fbd0781564" },
                { "lv", "077c1eb3b869e46d7458e200c64156db0536d62eb42c05a58ec9cf5e1880b730c3c56d7b891cc5078dba077a17a52b37dab5e30e0cb32f4b4fdbd87483658ff3" },
                { "ms", "dcca3e320b18f13d4f7a2ff0cbabe36be1a8cbba57b0bfa7e670fcd55b9b98ef78c3a1fbc2d9cc187902609622994150997b213eab322919fee2b861392f167c" },
                { "nb-NO", "c699fba44e7c721446c9ba699e849c74829619251aa810d6f143a75cdf61a09d61a08992592b78418f4c32593fe6807df88e1aae9592b65daebdcc115d501dea" },
                { "nl", "3e49d4443fa6000539b77cf2b4e6fcdaf05969ba01dac7b45b0fd9aaa7b098b43512ebcb7a60df82d70c886ecb63c6b51ee5a80e9589e4126beb0bc9d9176b1f" },
                { "nn-NO", "0973ef8a4a4566be2ace62f3bedeec63761d894ace0c20341b31c33ae639a0c6687fc63236cf6d366c5ee4ddcc11269f34f8b6755fa20953887b487639b5d301" },
                { "pa-IN", "fe4b50bce409fdb89fce6813b25765aff08cbba744a051e8e67eef268ef0e08502e2ef8941ebf8ed0319c2751b10ce96217f32dd83c7864f3fa14196f9bf77f9" },
                { "pl", "4e44951a2fca8298c8008859235534cb65a6ad69787fc940427a9b3eb70d67d4a3c90f3ffbc80e00859f7f271070301ea6190774615df2b872e9b450f861cea5" },
                { "pt-BR", "2347c39ea5f9c8a953e130d52e75b54c8dd81b0ad4bbfa62bc10d678b7bf82da0622f05292cda27a2685f6fb9e08c7c5bee25a4f769f6e09a08d97bccbbe85fb" },
                { "pt-PT", "889e5bdc78eb8223b9ab6cf859733e595a77e8735211378928aa25870a04a8877b931aeee0d37e4d4c41e5242900542eef65c47d3cd4afdc3253ff51596472a0" },
                { "rm", "08532ada5b43111475ca0a0d64b81b6dbf2d27d50fe7f6446108eddd681b5a91194f5b173fe759cb298f2986dc7844daa53746618f111e24431736547e755052" },
                { "ro", "6365464387d994e237f914feae18d8a422591b1d78bc17e4af15c8ac5d8ca6810c0d1b184673ba61ab5687901b8db9292bc3325d30c76343ada150fbd002dc71" },
                { "ru", "232fc6fcd0df7642e888aa0bf6c60693d37b697d8ebfcebefb2e72f73a0258203c3a1f42e1baa88a3fdb7432ce0f4395e6b30e1f19ccf0c51409997be6cd7328" },
                { "sk", "9a9ce9654652ebdfb556fed7e13ecd64debba662550c965db827a9374774d6590c90dda0325cfcf3971f860f400ca5f6148f8259f0202e19dd754519b172b641" },
                { "sl", "08958a47ddbbb7a9cc713e94f201f8e37855d3f711d2f03152025dd44e7e451067b3d1bb0dda4974e5ff7596de9bdbbc64a219dfef8bf7b12275d25cb5ab7e83" },
                { "sq", "8944064cc8970da385af7ae58d31db318f42345356db5ba58010672b235270cd5e4781169b76747078f0e1042d2f6d35a05fb7acada66b866c98090e8b7459be" },
                { "sr", "524c75e06ee37107b390e5250cd01946f3b5150d511fc400019d4a94931f808bd80104f4e232552ce53cd0471be16864ca42c25fe74f704878694c04186ee0e0" },
                { "sv-SE", "4a1a79a9535316f3421f3325c59abd6743c00d320a6623c80f235aa48e89a4655face96b182007402114adb34e0ffda7713f6ffa0b3567f6ab5a9141311303c2" },
                { "th", "f725de029445d26a939e623bb3e5080315fe2932c7709c3bf6520005446874c8edc721982fc52484f19b4912f18dfddfb414e9a77512327440ea809741a5273d" },
                { "tr", "33e0d9d2d7bdbb1be01b9c2135d229b91141243d0d903281dffc3bb5c18b7da0a50c3e82b60365013b2933c8f0f40d4fa3fe266aa372419d6d7a92a80c1b0d95" },
                { "uk", "5e9f2e29058bdcb840eb39207b89cebefb4135885003536d6f18b7a2c1fbd46376bad000cc1dbc531a75a06479cc11c3f9f144824f8951a9ceef84398342e854" },
                { "uz", "f20a0d7d8d9770bff555059ea484c45d7d56794939fdf6f3b9d7c2baf8b0cff7970eba855f8f72f6687543dbcee46c256dda8a0fbed539572862bb7a58006060" },
                { "vi", "693fa9173ff3aaa64535c09d3c1ae98b62fef11bd4b50786548310198fb81b3661a8dc7fd0d160f318d1ae018a15f33969becb0e0eed2e299776937fa9d04fa7" },
                { "zh-CN", "a3c66bb56c83f89906b297ca0d16803b41bd923cd3ab799d6a74b02a8b1edc8625d334c65788dc947510b5a748b317c0fede97da49d09eb6681d9f5f0f98d59c" },
                { "zh-TW", "33b50f9aca5bbd6e4b1388d27c476ce7344c00f3a7ed3e6f668d2bda7240509ca5ce0922b4d34a6f3c29aad46f61ba0aee54f7635248013f8f6fab6f7c3205d7" }
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
            var signature = new Signature(publisherX509, certificateExpiration);
            const string version = "102.3.0";
            return new AvailableSoftware("Mozilla Thunderbird (" + languageCode + ")",
                version,
                "^Mozilla Thunderbird ([0-9]+\\.[0-9]+(\\.[0-9]+)? )?\\(x86 " + Regex.Escape(languageCode) + "\\)$",
                "^Mozilla Thunderbird ([0-9]+\\.[0-9]+(\\.[0-9]+)? )?\\(x64 " + Regex.Escape(languageCode) + "\\)$",
                // 32 bit installer
                new InstallInfoExe(
                    "https://ftp.mozilla.org/pub/thunderbird/releases/" + version + "/win32/" + languageCode + "/Thunderbird%20Setup%20" + version + ".exe",
                    HashAlgorithm.SHA512,
                    checksum32Bit,
                    signature,
                    "-ms -ma"),
                // 64 bit installer
                new InstallInfoExe(
                    "https://ftp.mozilla.org/pub/thunderbird/releases/" + version + "/win64/" + languageCode + "/Thunderbird%20Setup%20" + version + ".exe",
                    HashAlgorithm.SHA512,
                    checksum64Bit,
                    signature,
                    "-ms -ma"));
        }


        /// <summary>
        /// Gets a list of IDs to identify the software.
        /// </summary>
        /// <returns>Returns a non-empty array of IDs, where at least one entry is unique to the software.</returns>
        public override string[] id()
        {
            return new string[] { "thunderbird-" + languageCode.ToLower(), "thunderbird" };
        }


        /// <summary>
        /// Tries to find the newest version number of Thunderbird.
        /// </summary>
        /// <returns>Returns a string containing the newest version number on success.
        /// Returns null, if an error occurred.</returns>
        public string determineNewestVersion()
        {
            string url = "https://download.mozilla.org/?product=thunderbird-latest&os=win&lang=" + languageCode;
            HttpWebRequest request = (HttpWebRequest)WebRequest.Create(url);
            request.Method = WebRequestMethods.Http.Head;
            request.AllowAutoRedirect = false;
            request.Timeout = 30000; // 30_000 ms / 30 seconds
            try
            {
                HttpWebResponse response = (HttpWebResponse)request.GetResponse();
                if (response.StatusCode != HttpStatusCode.Found)
                    return null;
                string newLocation = response.Headers[HttpResponseHeader.Location];
                request = null;
                response = null;
                var reVersion = new Regex("[0-9]+\\.[0-9]+(\\.[0-9]+)?");
                Match matchVersion = reVersion.Match(newLocation);
                if (!matchVersion.Success)
                    return null;
                string currentVersion = matchVersion.Value;
                
                return currentVersion;
            }
            catch (Exception ex)
            {
                logger.Warn("Error while looking for newer Thunderbird version: " + ex.Message);
                return null;
            }
        }


        /// <summary>
        /// Tries to get the checksum of the newer version.
        /// </summary>
        /// <returns>Returns a string containing the checksum, if successful.
        /// Returns null, if an error occurred.</returns>
        private string[] determineNewestChecksums(string newerVersion)
        {
            if (string.IsNullOrWhiteSpace(newerVersion))
                return null;
            /* Checksums are found in a file like
             * https://ftp.mozilla.org/pub/thunderbird/releases/78.7.1/SHA512SUMS
             * Common lines look like
             * "69d11924...7eff  win32/en-GB/Thunderbird Setup 45.7.1.exe"
             * for the 32 bit installer, and like
             * "1428e70c...fb3c  win64/en-GB/Thunderbird Setup 78.7.1.exe"
             * for the 64 bit installer.
             */

            string url = "https://ftp.mozilla.org/pub/thunderbird/releases/" + newerVersion + "/SHA512SUMS";
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
                logger.Warn("Exception occurred while checking for newer version of Thunderbird: " + ex.Message);
                return null;
            }
            // look for line with the correct language code and version
            var reChecksum32Bit = new Regex("[0-9a-f]{128}  win32/" + languageCode.Replace("-", "\\-")
                + "/Thunderbird Setup " + Regex.Escape(newerVersion) + "\\.exe");
            Match matchChecksum32Bit = reChecksum32Bit.Match(sha512SumsContent);
            if (!matchChecksum32Bit.Success)
                return null;
            // look for line with the correct language code and version for 64 bit
            var reChecksum64Bit = new Regex("[0-9a-f]{128}  win64/" + languageCode.Replace("-", "\\-")
                + "/Thunderbird Setup " + Regex.Escape(newerVersion) + "\\.exe");
            Match matchChecksum64Bit = reChecksum64Bit.Match(sha512SumsContent);
            if (!matchChecksum64Bit.Success)
                return null;
            // Checksums are the first 128 characters of each match.
            return new string[2] {
                matchChecksum32Bit.Value.Substring(0, 128),
                matchChecksum64Bit.Value.Substring(0, 128)
            };
        }


        /// <summary>
        /// Indicates whether or not the method searchForNewer() is implemented.
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
            logger.Info("Searching for newer version of Thunderbird (" + languageCode + ")...");
            string newerVersion = determineNewestVersion();
            if (string.IsNullOrWhiteSpace(newerVersion))
                return null;
            var currentInfo = knownInfo();
            var newTriple = new versions.Triple(newerVersion);
            var currentTriple = new versions.Triple(currentInfo.newestVersion);
            if (newerVersion == currentInfo.newestVersion || newTriple < currentTriple)
                // fallback to known information
                return currentInfo;
            string[] newerChecksums = determineNewestChecksums(newerVersion);
            if (null == newerChecksums || newerChecksums.Length != 2
                || string.IsNullOrWhiteSpace(newerChecksums[0])
                || string.IsNullOrWhiteSpace(newerChecksums[1]))
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
            return new List<string>(1)
            {
                "thunderbird"
            };
        }


        /// <summary>
        /// Determines whether or not a separate process must be run before the update.
        /// </summary>
        /// <param name="detected">currently installed / detected software version</param>
        /// <returns>Returns true, if a separate process returned by
        /// preUpdateProcess() needs to run in preparation of the update.
        /// Returns false, if not. Calling preUpdateProcess() may throw an
        /// exception in the later case.</returns>
        public override bool needsPreUpdateProcess(DetectedSoftware detected)
        {
            return true;
        }


        /// <summary>
        /// Returns a process that must be run before the update.
        /// </summary>
        /// <param name="detected">currently installed / detected software version</param>
        /// <returns>Returns a Process ready to start that should be run before
        /// the update. May return null or may throw, if needsPreUpdateProcess()
        /// returned false.</returns>
        public override List<Process> preUpdateProcess(DetectedSoftware detected)
        {
            if (string.IsNullOrWhiteSpace(detected.installPath))
                return null;
            var processes = new List<Process>();
            // Uninstall previous version to avoid having two Thunderbird entries in control panel.
            var proc = new Process();
            proc.StartInfo.FileName = Path.Combine(detected.installPath, "uninstall", "helper.exe");
            proc.StartInfo.Arguments = "/SILENT";
            processes.Add(proc);
            return processes;
        }


        /// <summary>
        /// language code for the Thunderbird version
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
