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
            // https://ftp.mozilla.org/pub/firefox/releases/115.0/SHA512SUMS
            return new Dictionary<string, string>(100)
            {
                { "ach", "aade40e40876e0fed5d20961710942aa02ffbe054ef5c36da17c256a5eee2b899ef928e05e1f7726c403d19638414cd066dcda635b52b5226a4954aa51907300" },
                { "af", "56b558d4042dad984ee533e455f60bedd31a09d7a21eec4f3bed6f8331fd4cdfe12725b710468345405277f04088082cd908e068bc76a509d9fa642f2b6d5e36" },
                { "an", "5fecfc1176cd4f15193c19f8aaf47c1a9ad39be9a58014ddf946493f89ef78b0b48ba1a7372cd7f2dc0e14b774a33e990c20d7bdba403f0e8cfd03bcf8b670c6" },
                { "ar", "dd8d0c3db207cf31c8291ce4f8d144e610982a7a4962a8cb97cafd95ddf822057f5a381829bbd31950b737bedde03dd40509ccf665aed2e3cffe40fafa3eb3be" },
                { "ast", "00736a5bc6bf178f530c937fae4e2485653cb91b00c4870ad15e624ce1561c6caf278f372a84b24164eab293ae16ab44bc6efd92ad6bf98f5c82498411e920d9" },
                { "az", "234be1b79356626d35a685e7ed16ef95e5d6c6757a4bc04868a5d9d6c8ed2a1bc7a4450c9e307fb245580f78a71ac9490e33d48f59c8adedbad5a620f2908b9c" },
                { "be", "ee992a1f033f6a3dd44d0934beb43798003dac251b02b47fd01773fe8d3f2eb6d82fe9949a314bc63d1b3980d2198f46964c8dc032bcbb2e80c774376765f8cf" },
                { "bg", "2348dbd113b5251a5ede0caaf139819950820874e8bc951760e5a81ce22070aca3953cd43c832e393eb93c2bffb2b2695e912fdff016988f2dd27bf30ffa6aae" },
                { "bn", "8c1a0123cf76459ccaef607a88b915aba56e750a6bc789c727a59144cdff4ad934e19e8987aae9d95c8e6ca8b019965e391ba040d26f7589faa5481407ee88ca" },
                { "br", "95935d22615e5c52ee302812a590e4b58fd676d437e942361e7742b56f9970e81e5d9a7df0d3b3540af37858d256cc6859e09907f72f238cbec58ac6f5a14a01" },
                { "bs", "94debd955125249c23abce671868056fd1239cf0126b53e622a78eecb61e3ed2a9d8b9de676e4a1659215a69e078e820a73224c3f0e3e1289b0ff3b7dda9c610" },
                { "ca", "82882d31d592c2be9a3ef4b8d429732796de9d3227a228f4461b44259db95ccff83529218f4ff9cda2ffa223873d0b7bbb2efd15bc825b54608043a92a5fac33" },
                { "cak", "36c2b585e10e9ccbaae81bd7a59f71341d9f83053cb8a406e56c184df01fa21e0c92653122d22fe758255ec97b9763e584984e4ae33b6482aa0e61871bde514a" },
                { "cs", "c974c8933379be203ba21446c82bbca120b8b99b5d60a50e7aa5e24e207f78db51e7bdfce40c0b928616f82d12a4ff462fa8da2bb887e0bec3f55595b5bd8c8d" },
                { "cy", "30fabf0c1dc6470563a96a3432cd4112fbe0826e6c5ecf245e7276786f12e4067f5fac28c93b2f0e30906c6887eef2c39139d661fadd9db1c5ea8acd77563faa" },
                { "da", "496150c7443979d72e45b3f1bf5d0588de343e2a45ec30661d8e396590c9b9667ab588bb9673f4c29ada5c918aee72cdbb2f930d4aab62c811fd4bf44e0085e8" },
                { "de", "4f42a2141194e2c1ea0917eaca452d979a86f0ba5bf72880ecb81fe77c2865487d58530c745e5ff74dc5318d6b7bb269d5be52091194aca05c7c4477b9aabf60" },
                { "dsb", "ff27dec6c3e2cb33c57b9ce44f73550b8149c1649b4b81ed6c0afda481c468ed82c6c736213beb2f2424b55d41a4147a91e61d84baed446ddfc2d609c81f316c" },
                { "el", "5b57aaac617eca5afce73b0ea0a9638be43f1b541b3a561eb3c2fea74ed7a76dd6448ac956da586cafe4fcb7de393b391afcf5f924c45992ce104924510c5d70" },
                { "en-CA", "4a719b1fdb039d4a9408b2221de351d955b0bb5696c33a6492fa9d3a1f58bba8043606ec378eb63a33965b9cde20e1084a408194850f80747c78caaa489e3eab" },
                { "en-GB", "269473296a411ef2fd0f7a6e13ee9a5967f23146d13838b60197182f5711a0b4e139cbe7ec508d6ae0271cdc13e40fc83e62f42b6c3d3fdcd6bff677a3421fea" },
                { "en-US", "735e6a564c76c36c1528a58dcc7b1ca65c0aae379c37910a2e7e9ceacc6633711cef3f94bfe2cab85b2d771550b7e35ed602543ef1aed602ef814da9791b527a" },
                { "eo", "90bf0df8eb1928103c39d3343057829a778af9b21a45b049f1e6731ac0e02e8e11ca513201d569e0d96c6c6e40553de2ff02f7164c5134ddb898e4c6fb1ab3d6" },
                { "es-AR", "b8cee41d8584cef54800bb9551224996be33b92238a7c48992d14b0146dc821817118aec8be6cc9ef585538b901c75029c5933641b90b3b58cdff36f7bd9c13f" },
                { "es-CL", "accadf11d40433ba1e6fb50ad9a8b820f3ebb9d486658d88233f89b17b163e4a76596ba002770869c2faac8f029f9fa0390da9beef3405f6c32f30d4059db98e" },
                { "es-ES", "aed1b0f91cf48a87a13334d995929ecc215ea31041cd85c2b2534156db39d316a1c4df404adaa6b52b1a18b0c7701520efb79983b96b16544ddedd2f8755080b" },
                { "es-MX", "a2209468f7a02ac158557190ae0a8b37ea4cc5ea5d0c54e6ccadc45d0f6d526bdca09d7d86da854f2f80009d17a85560a1712d81d62c68348e219aa9958a7280" },
                { "et", "2820f5fa5053991f6123c7c986c73344d1ec700989d66aa359811d36070f966df21505f6ae4ef3443f8057b822eba77d6aa0692687e6e5ed474525a98b92ba8c" },
                { "eu", "90a3878549ca8349e97666dc6d4f09073453b26909ed72e8934bc2ffaea309eaf0ddbdf3fd2561f9dc9fcdedeeb0072c1f606520fa6830a94516f7e9b46de55c" },
                { "fa", "3133c71288e74781ad2f50e30e39a75292a8563583a5f9fafe71e883462be3e2f5d4de067d9055ce6bf8d29dc0bee013ab812f84d91c4d88fc4e8e6b32b46229" },
                { "ff", "6b664ab75bb980fe806e25d5034fb57e0b36f620dfbbb5f39760f579126105e58499ca4137df383af1a295ac5d6a55dd3bc96da5f85f3735216b407ef512cd30" },
                { "fi", "535292b9036c3b6a98b965c0ffa6d3cfecb6b7a46201b4cda5c6167ea5d470f3fa1f7bbb7604acf65c5b2f82ed8a6b9da4358ef8cc57aec5efb72afc695631d8" },
                { "fr", "fcfca98ca4906c84cbba1fc2b4e5110a733b5cba7921b1934c3b4a5fc4a48157df3e5abdc95748940a32a458e1ce4d8c9091234ec1f4092b2d1b2b12314bfbbd" },
                { "fur", "b10ab70afb036a117d9ca9ac0004640f4ecf1f1e6101809479e77ff986575aa24ed12f11da553f5e836be170412aab1bd49641f47e658a8760a568397628c947" },
                { "fy-NL", "c477d4656a6159d437c1a8ba956175179c42609b19cb5255cecb76d410f7ce61acfb490131d515cfe6b8921b0b1e09f53fc1851c95de6dd9144c4cdd8d582a08" },
                { "ga-IE", "b4c20f8ff1f0d1117bac1891ff11bf51faa21bc462112f26346cd79e10bde0c366ae0e3b57e3654dae5affdfdccfbb99ad8d282a8342dab3ad57ccc5cd543158" },
                { "gd", "7c4cdd9546b4d101b23c2e515d2eb3e9a362cf8dc68bdf9397dc3c33c497141de2c30288ab34c2a85803d1bfd2daca9358b3b578154d5601491213fa97efd25d" },
                { "gl", "ac3436a05e45dc09ff64a65d993cda9fb5ee35d80779e4dbf3ecab9728f2e33abb0385740e786a65188767e45a0bdcf271f9140eae1b9f70910492d098138ac4" },
                { "gn", "efee44282264e5e08cf801de2e54a1b3b2008bf6c57c716450345cd963812119d9761ea6a264c56dfb36f149bc7e5b24a7c8f8fd6c44c66b5d7ab4443552353f" },
                { "gu-IN", "2c7659fcdd9cbbc70bd6fa24379e041738d7d7d867ad4185fa2e4ef39559b7f6f0c6fef2ab55761bc05809273495b3fe5ed430d2ce3414b839ecbdcfc3aada7e" },
                { "he", "d75d7ecbc853ef6d136e90545bca37a821acd18c691304c77804349dfb54c917482e2096d82d188a4fb687ca1c778bf510edd5cf0254a4bcd223c04da8816c04" },
                { "hi-IN", "169ddd510e1c748e19aa0d4288470f6e877658529564b2fb5da276abe7fb60cf1c4038536304c0bcd0d8e6153e4bfe0c6fa7a78cd935526afac0509199870e61" },
                { "hr", "22cc9ab1651f75392830c42a7021713c14195fd6cd677fed0a92496abca8f441fea53a095ff70f5459b54a6facd80b8452b5fcc25d97a1f1efa43d7076e12cd4" },
                { "hsb", "8699c68b268137cc3c8b95db07cac02a61c7fa084136ef5375e2e02628b376576199ce44ff34dcebafd566d5a30ad289d06d33281d890d75ef7e41b79221b2ab" },
                { "hu", "f86799c7a01a1038e367a172acf75ba247c680fc299ebe392429eefdcea69ccda254c9b34f3db7b0d3a9de21c5ad63b466b543df87033781a01b417b5aedf408" },
                { "hy-AM", "7dfaf7f0cfc8fc19be4622927f5bbf4aeb925553e3fde47ee7da48be6b16856c61227b22dc11c1e9d06fbf9ca13fd2eb8830b6394bf599b4b4db1780f35f35d0" },
                { "ia", "00d819db56485a78dfd76f9f1fee1ebfeadbff578475e946798b53fc1118f40524184342d0bb18fb269951cb592a3df58c241f5ca0a4add49362b3ac2b5a751e" },
                { "id", "40d68cc300ddc6304ad7f36b9ed7eb8714c28b866daebe10b03d42edf0d6edc004094e4dc090f8e7e4f4ecf1cbb67cc5ec1e63288aab8cbeafbeb14d7dac0878" },
                { "is", "2134891c3e9197a52c4e68bc94afdf879701f636a2faa9ddf7858e27575c49a0d4b448f615f68c7c9d075a96fe8d6662220052bd077f3e93a675f498ca06b238" },
                { "it", "56a928469ba56c2f6c9e8af37394b0ecfca0a2bec7f6ade902b079ad621449dbc687d065cb97a4267388e40f512edbb474846aaf60adb33a5bca386895bbf6f9" },
                { "ja", "19d9fb86a85289895c3f2bba612c550604fe74512f2a96fbcdb72e0b5c586e4a278ef98acbb7d1f2661f777fca3c4013478da31d87dc041305c404b595df0def" },
                { "ka", "743ed25a752b52e584e1073d423bf2adccfe0befb2734ac902ddad224f622847a7a1889ab52da46743086cee13c958d9ff0edd90ccc4750150cdc8ead87c0850" },
                { "kab", "dac7b1d6abe609e5a613cec933fe64f0914988aeefe56d77e1b0ce49482efe57e2b0b8148e8422baddf6efc76619a69e1e07208542b55cd997cd39c4cde14ddc" },
                { "kk", "8f5b02d600a575e881f3f27dd6f4f1d045bb00ed7f26759848a90e9e9524ed9ca41a264e7a82eb3124d1cbd5a8510aa82f58f36ec59120ce19a434480570e739" },
                { "km", "a12bdfb0701ac490fdcde02ad1e94be1db4e5abb556efdeabd4faf986b6889d430b3880d2f8e5a209a805be3fc25fd82ac98431968b82cf8f0e07e76c13a90b1" },
                { "kn", "50a998239edc3a59276dfbf920f0225c8177a42176a3e45c70657a406b7636aa7ae5b3bfb4cb57834177b2aca95bfa7703350d3018ac875d6e21fa2e3326b2fa" },
                { "ko", "0588c6bd27b88cba8473cc6b3f9d5149e4d403ab902e5b2b29636e3bb74ec5b10ee542949ea64b43a397697523f4516a026503552d96cc69a8bc3ea445b94a11" },
                { "lij", "1bd9eb3571bda6ca42b769131ff1547703a354f08139ad2532d8a11eb628e1c2cb8cd4beaff4538fbfccf508cef5506716928e32f17aaf9eaf4d75bdefb4f3d6" },
                { "lt", "03080cd7385c2a8c70c7c90bce2af1439cf55af1d90403489a8e76e773a70a768f32ba9794dcd547ed23d187ec28b60d4f358455416c394f35271de887218c91" },
                { "lv", "2767340ec0dd8833c993917cc3dd7001fc48bf3bbd14239921b1180aa6b2cfce7b3f91e73d22d8e015c11b6135c575cbf200b3c24ce79ce124c5e6ddf0bd6adc" },
                { "mk", "78b087a3208f26144e57bd275224dc1e6186cc6008dab4ec6a1974990875594092991ad6dc372410565783f87b22bdfbbf98942d96826990ff2fc9088b584135" },
                { "mr", "f112ad8beed9468dae224e9fea4e07e01caf3aa7cb0c88812e2d3e4a62dfa3b1ab6be5f01c58226ba7a4b4284d931c90be91c9e690e81ca2a1def356d14c9867" },
                { "ms", "733ecd6a8a634556e1425c97d3e3e1a77a8da7c81d52508fbe9580c7ca91c4d942aaeb747ee6779502dde996a97f9939ae9b4f5da890f65717e4bca129ae99e3" },
                { "my", "556e2e2dc14de31074fc9f389ebcfb97bbc580ad8769e704508713c3c867537d16e3539bdf44b6b2b281793992da9e224aee90e2b9f40e6fe9b7944015ed7981" },
                { "nb-NO", "22879e4c12045d42b60da638ffe873cc731e49376997c93219066ff1cc5db19c048c36483ea18471b02423df1d23a5e18fdfc5c6c13844829fc5777c018b921b" },
                { "ne-NP", "11c0e39f9d896f1ef6c06a2165c95c7d0254775ea08d822f50c4f49df7aeab2fec32f05d7886386157642a345ccbfd37bb4832f9d2e5a6a504bffbfdc3592970" },
                { "nl", "4df9c37e12466a7aeab6087079884772d84439976382ec407e8416eaf7c74255e23612a6441489b0ccc02c2f080df469fb0633d3facf9a551d518563f8a1aaa1" },
                { "nn-NO", "7bc6354df67482ba0667911b4df85eda226f3ffdc253a346951390e7a4ddbafc5203f0bf8ccc791bfc9cb56f78967ed2424dcdb0eed64b3bfb357ee2fbc9ddc7" },
                { "oc", "5a934ce6d102a9cd18a37112cc9c1782414cb2ef20375909286eccc4a6c9e0be0f9eb03d608e3163538c559598dd616c18222cfd2c1c424b3090b98b2a5ad481" },
                { "pa-IN", "a3c3273f0a8224614795886ecf52acbb7cb1c85a0e873d885c5b38ce02638ab2181cf01d98d8e1c651f7442fe47fc74589715608c3d02f8297a0c96331de563c" },
                { "pl", "2a990d4db992c88914d3fa80a676027a69a74645500fb0765f1f6356a6a850455343b4e9f1eb91c6920712d62b716641d5dc333b53e9de61b0bd3057c0e0512b" },
                { "pt-BR", "26d00ad34f32a1ed8cb38094c7681a2f259e1870a00ed47a7e1b29a430ab9bf781f45d8bc12855adad482058837a22b9ea33a2d3cd2d6b7ebab2d72d55574104" },
                { "pt-PT", "32ff98b94f19eff69cda2a159a3c601352a397a7202e9dade9255d6193cdeadfdecc154d09e1240a3a7a1782efaaad49cd0d79f3a3de1904b2388d3e0e60777f" },
                { "rm", "2a0612a492fed44386983419eaf196c8456b2cbe4407d1616487050dcb032f949bea6973d9130ba3dcde6a02a9c2a904c0034b9ae155f57a4786273ffe4c6305" },
                { "ro", "a168f2b0fa4f6510ca0a6b6caad8a51d31b68c5f318784e1020043eb2aeccd500a61c9c4c511e23f28b29e72248415c527efc1101ace3891720f5fedf80fc883" },
                { "ru", "dfe09321006a41cfd898c6b4fe936f949966f2b1f21b458d4e4edd69358a66f9da411723c49ad285d246421c0ab08260de0edfd5c94a05fac393501c8bc0c069" },
                { "sc", "562e3ccfa91bf71f85d1491d30efdf33eaef8e8e6ea842e025d7a7e536e068d2430319df1c54a97e1167f341fd01569f6056439ab1291cb259ce37018b19ad54" },
                { "sco", "38581ba4caef30ab4177afae37758e54531487ecea6c4fe4737d592d3bbec1ec09151571f0969fe28a477cccd4b5f99454f527e1c20c6e442a32807357d41582" },
                { "si", "b8cc6401585710768081de528acd7587c7be8ed833e8618c687951aa809c6429922507e5cf13e6c475c448939291bc2402a8e6f10e3b2bcade4e4ec3d8982c23" },
                { "sk", "55c46ef9035479ecd76c9c543cb7bcfa196e5c75d462595e54c04827c8e6428868c6fb47bc1dfbce2526f8643146e55c623efd9d715c16950349141276dffbf7" },
                { "sl", "d5f1e66f2fb9f86765a29c820b248cf63ee9712ba94c8a308c926dd135c2e74971d83f93e37a93acf96e8ca13de7c7e840e7e67e50cda0ced7a9cc0d4ff54794" },
                { "son", "143fc3a3ed3e32951625719d4aecd6853746fd87c4833f0ad434eaa6e37bebbce9aba095b08b59b08e9ad51549948a7d1dd450b2c79b95b02e3678e41fe2bbb9" },
                { "sq", "46741deb421428524344477cf75836ea3a45e47e608c6fac625437b6e1bc17eb39b7555401a6490adde1050a8d5cc0c365374c08f5b7b993b3eedbfa6e53a467" },
                { "sr", "cc46f3710b6b6d69a36e913f98f77b2e929a92a829a6f3a4b13e5230abffa747544946247472099c3776c5bd69becdec6959bed433273c7a60ff046eb99c9639" },
                { "sv-SE", "a51dd6c2303407eca7ba8bdd10ab1c199d02a1d9b09d1f83e79144e1a478f8f76921833981d6bf1a3e9521be4a725c401afcada2b717c4cd708cb0e9272a8550" },
                { "szl", "12ad8c92ec6a0dd1ad71e099e8acfd0ef8a2b4c1a19ae2640a686bcd3395576f1b971ef92625e0755f27d5cb16864c62868f308424b1e8aa5d03059f5fc78fce" },
                { "ta", "5b9e17b80b4bf0b3514dd9c40f2da3448621ec1b3c4b2d66bbcb83aa7273186795fc68845d626e3d277434c122f6d637e2046abee7e3039e19cef4254b253a08" },
                { "te", "484e7b378a1307ecd34ed0463eab02139a73ac5ff81bc6bc07236fc0d071d12ddfeff7104b3a938111ace43b77878c9cfadd950407b5b0d44fe0ca722f89675d" },
                { "tg", "703b6298831d5cd701e2c889c450917d28bd352375e5e68b033e872dbe3f3be739ce6b1e2b92a03ec4b5b2f395c27b5b5097d0a42fd4b3a6a734f557bccf1d83" },
                { "th", "66dc30e8d3f26f830f735f92fd9a8e9a2010a340448e9b42cd35833c334058f8a132b881ff8396ca58341a3a5042fbb4897e1dad018b9b916af7681298e381b1" },
                { "tl", "3c2f47b7578fd9d760f10dae75db02addfe21f524b472c5209edd1d5c4c7453a49954b7de96b3275b3879b85f3fb9cb381fe981898f724eef3b55ddc7818be25" },
                { "tr", "11df5b8409c88d93efad01c3654283e34444d7787ff609f124c186944f1fc25d68ebc8d08a4e3cd83688dfe16935d156be4023d767142be0c70cf626f80027b1" },
                { "trs", "72b2a3be95f51c21a87673fcb109fedbc0e096c5488a156349d2a7f7c7529ce2e905aaa550b5af78c0628e8d92bde34a37f2e3b5217853f703594f4df373d159" },
                { "uk", "36520cbb2bb079baf077ab99d7e148a1a519deccc2b69a906c9671f36bf5da2d094e833b7cc7bf7bcff5546d38d13263b38f4febc8b0548b2fc286be1161180f" },
                { "ur", "cd871da683f4bf564027999eea296005d15e7884e48c3c4fa1cc9ebb02a7726d3e78b269f6e7609d3b18864bfa388467f1d86302453a11ab1a673c98f7079e75" },
                { "uz", "0b6c5af54a5e9889edf954e4db23cf7954cd34f107c6e812596cf9fc453f0086685e882c39977ab92e3aa356413d24b47da203ec87855cc2a1f9c017f6bf499b" },
                { "vi", "fea238300e3d0cafd31d7ce5041619591e1f7f52ea98f0a1367a92544b806c746df4275c2a5455051fdeb74626ba971744fa694bab3dbfce20f69fa52b5d5104" },
                { "xh", "da0a2644fb10d019150431e5980d915eabd4819f69a93c4d3e29fa06960ffc7e9b871f383bc07342864de15a95c8774d26520f61229bc801924120ae7a038eae" },
                { "zh-CN", "613a56ecbd1f60ef68316875507f1a3b9a06d7b0c06375808beac8bbf8c5062c50268b7ede55afeb1be47c4b3d58a053a5d2c55c98d0ddd19d887c1c100a096d" },
                { "zh-TW", "522c351efce371243cfd172947c0f1552b5f74bec49475d9e44ed40c34c9046a533fbab9bf0767435749b19019def1a50720e9342e327b28a5348c502b3b46a4" }
            };
        }


        /// <summary>
        /// Gets a dictionary with the known checksums for the installers (key: language, value: checksum).
        /// </summary>
        /// <returns>Returns a dictionary where keys are the language codes and values are the associated checksums.</returns>
        private static Dictionary<string, string> knownChecksums64Bit()
        {
            // These are the checksums for Windows 64 bit installers from
            // https://ftp.mozilla.org/pub/firefox/releases/115.0/SHA512SUMS
            return new Dictionary<string, string>(100)
            {
                { "ach", "71672ff980e533f53c8a4f6cd83fa41964d3f7509cba2786177b79a554cf936be2c693340850b4733f3fbc2ebf9b3dd1123f2da1ea8b9cc88661b1ee83e81e47" },
                { "af", "6c3ae2de8596a8b6a1e96ca9c6ff331bc20f0ddce7d7d0a212b95857c849fb92edfd092ca246ca4701fb546f6f112f81a2ad62d43c7f29ffb5732f508e7ec3e8" },
                { "an", "7257e766df91820b81d8cdebbb57c56b37ff0d0d6d7fc915ff952be5620988379f1aede3de1c7055b1f8575b463026cfd88c81781fd47bee797ef963a72d353e" },
                { "ar", "26de255c604b2c5f5401cb7e8de6dff4525bd60e10a651201fcc999e760c75a91a3d174aa2e0e639f9808d16a084ae65f27168c4bf7ccd2495eacac4ce69a35a" },
                { "ast", "b117ea53926eac7de8720518c078d80ad8b9b77dbd7eff4db02ee3d65eeeb1819970dcadf59861b6baf82a0188557cc83a897ac32e1b727bba58cc4b03fc93ce" },
                { "az", "3afb2ffcf1a47edef43e107b19c63d1eb5d90ef057bb3e8119dca06760d9e2a7e338eb52420adbeef2fbc96259c335fed2a89a6cb686193842a349db057e81bc" },
                { "be", "7f74a8ed56cb1d927bf24005192e4fd786d52f024fe62a7f3b3427d0232a6b1e5f20cce2d44ea2a7406ef3f646878c3c48c2f17e8123a603263e295535037585" },
                { "bg", "4690241b336f1e77fea93aa1f43d5f8720c26c9b826c5d49388f5b8b4a1e231e1d071422c4c17bdbb20ba034726a841fc1140cc8a3834ebc038c78775097ee61" },
                { "bn", "3a0c81fd59d52ef3bc6e5ab69b2315e7eba65bb9436e52a6c86e45dbc74bf5e169416c71a5891ed6ab84da388e143ef6bdac70cc5e1f655ae41c82887db9985c" },
                { "br", "51e3d03f9ae15d6f9ffde8ba8ffb169cd07c3423de2e84d4dd780369cdf46ef9ddbde1bae74de3349e738644256f8e3a71764d7999717f3424a5a6bca0973ec3" },
                { "bs", "0865a2f285c7fdded7b2daf9d905964f26ab1b4569a645a933b1ae107010aa0acd8c28383695bc0082e62f9a6c917dddd1542adbf47f0c0421db3c459b1e79ec" },
                { "ca", "9e7d7b9fa78200f58f5c12ce62a61ede71ac2d27b37b16876da31c45313e906eb05885bad241040bf9d835b33bd2a667fd35712c97f7c83270cd0c521e2ce45b" },
                { "cak", "aec796975b3377087ff73c0b161ac8a69ec18418b48a9597f792dc8ce068a668feb3eb6ea9552bfce105619195d772cfe2aa7ff218ec34380360e80220ddb87a" },
                { "cs", "779e0e1fd2b21fe9159e5caf44f19e44efa1acd2dce2ee5cc65ef81586a0b286c2bdb73a43ab040969c8c08f2e3ab56e548e9d5140fe099d09e52bd521cf792a" },
                { "cy", "4082cc4ad40084e2f740e180a64c3dfb8da6ae452bf505d3dc6254ba0570cf9c4516858d212ae6ae6c46fc59ccabbc5518fc01d6334220d29065925cc5ad20e0" },
                { "da", "f9f7a8a1e6310b9454f8bc91e67b4ed66af533c2fe427dfe32a139cb709c318aef74280278c9c1bcaa4cd85cdc17f326ef936aa4a57f497bbb68838e1fc5478c" },
                { "de", "e9372394e0dba47a8b48d3b88770080769a90d4563389cff342293cdd57a9c208e0685faf7f68ba833197d1560f4ec37b9e86500425e8f8a65fdbe560918768b" },
                { "dsb", "e157ab0229b320503a2060edc4a4d12cd655c9e10f3a4e8fa0505e2e2d846cd2ee849aaddac84bb7311273aae280990c590bc7e72ad8945d04d0f1e76cca8cf3" },
                { "el", "a5b3b8f8ef969cd4683f774acedc2195f7979938e5c206cb1b99035a7a6faffd8b9d2f61b52a7b86a5ada6ed4a85e66b09687202db068b7f81fed97b98fc129b" },
                { "en-CA", "d05a36abab91499cbbf17fdcca127f2413e6224af08c9110c7e27475d18e48dba80502c1c2ab1a1bb7daa2a2ee2f4390f4c33b386fe0191bfde4a9de08933d50" },
                { "en-GB", "1ea57d65c3e9751ace14b835fe57f8f1504fc068554c9621d4bdc430f41bc7acf8d567505f08518fcb0ed2ea7ab557268ca0b05e7cd6316f7ffcc85843a2de10" },
                { "en-US", "9ceb37b8d08f9e07eb3235f7cf454deb0c815c9729c608ebff3bab521ac4ddc7f258cfeec49de78dfe560500f9ceb331715b8241f01059e3e5964807f83ab0d2" },
                { "eo", "98015a3f37f72da0b42f677cbd563e078c6c86be9305e69749626c10aa4c231b1e7d0e85fbf3aca02cffd3df03b61480b4c5fdcfd4fdcf94229bab5f27728758" },
                { "es-AR", "9e018d35700ecd91a0d9ba4b903334c5f7abc35c57d7d58fc59ee66fb27486053d1cc8cf2a6ff820c22e5133a8d3665b36ccc8397d1f5dba9da22451306d0ed2" },
                { "es-CL", "01266dc652fca2d0bf3f1d83a6cf949f906f394d4d276f38711fb32fd211796a40e582878b162de02dde95d74aa7ed28d44b07c52dc372c392581f733174f342" },
                { "es-ES", "0e4274562293028fa500fa3a1ca42e2bdb837011929af3677c72b2da4fcfccb5ce578858e574770c63bd70afcbffa54cc70565d986fcadee29efaa05a1cd1d72" },
                { "es-MX", "90a9c761759adbeaf8546efc7c07633cab53f757c889294e58e25074f608ae9b6bcdd89ab805783bda1c35e44451fb726b7469ccd418bdbcc6465cf0243a8f4b" },
                { "et", "6377d922e7d06b67307c2a38f4d7a32e699f883b50ea358e3543d6d4e44e2b5a13c2ce839d6875354aacf05157c1c1c4364a73bf1d62c4144e982ba77d55b4ed" },
                { "eu", "09929bdc5a4dd82e9a9e55f643e28b8de71d1e133608d7b9e9c917a8e4948859a85a981a4278223713768e4dced7440138e75a3d5c8751e748640c12cd2ded67" },
                { "fa", "4d597ea44bc496de77cfa4f58c961bafe21becd69d5a9b557ea7b642fed9921a41c1f63b7faf3c0be77d3e3760f14fd9ea882f5f4242b8c5c18e8a06a9ffc138" },
                { "ff", "8cbe8c369780c83ff19e8b69d677f04953b1fbe3772410754d466358dbfaaa69a2d0ca238226237d977d0336ce4ac2de47caaa011f98b29c182589f24befa3ee" },
                { "fi", "79dc7b3b91f853222af92f4ede1847d3222dc4a8e4b3408e2b618f914b26428de59bc930dca6a575fe43017e393f95e790b4161b85f660914d2cf8b7e9bce07d" },
                { "fr", "7ce68fba97c116cbc14cc38182f393ef98e78b40da93cadb3b5f6835914dc77b788726d0458ff11a4dd28c37281ed1cbd9ac6dc2862ea3a431d8b0310114c11b" },
                { "fur", "798d60d57ad56b03ce5b29410acfc96ea8aa8ff3c91e0ac17ee27fe98e7e719ec93648364b031e3da4ccc7eb2edc228499a2c013d9d29eceff7718e4ccd39d75" },
                { "fy-NL", "fe8049446169bf064c59380f13700fa1257f33362b8685f3eb0f52ef5b7cea3f8bff2a919768475948d7d321e2f2064808bf3f3f19089e74b8414b8b125a000d" },
                { "ga-IE", "7dc9a67d3d7eeeb1ede34d8a0a63998101c322c991811b49ea1524c6082a756ae6682407fb3c45d7f7262198d9ef67d626914a23673777379127bad9e21637a0" },
                { "gd", "5fb68f258977e6e945ee35328214d82cc5c9073a9c1ea3ed7b493170d8479f3f686b13a44ae0fe6aae208c67fc7fc8ec2661907b0b627bd197cd6d8e13d3de3a" },
                { "gl", "8052fbcc2305f7d38016902d46bc999710f45bf3e6eab7b9776cecd54b9ab0ada65677a3003831c3c9d77e1be2645469fc41f292c24ff673b73309e12a5001d0" },
                { "gn", "d078f8174d364c0f2a8e28a8038dd0e578278f635d72698dbeda27b518b120cd953e6f74ce048ea79fd4e70b3dd89c709804a5f4ee2006707295988c78798639" },
                { "gu-IN", "e54a4079530d3884c2276a2b088d2cf488751a208682d04054e91265fae20af39aa9e028f76342ac630f52e082b3228495d481ceae2f628fbd2833bce644478f" },
                { "he", "dc229f7a9a2c6519a6cb79cf53a0e861ff5ad79416fa8785ac2183e672361255260ce92647485548184c45d665e2457d00d684c93e22df18b22791181f117975" },
                { "hi-IN", "255c4a351339e180db9cb9418f091911c95a451558509b416912c79458d9a097e580d65c7f61e702cf4a09d26e83b42c591271adccbd0619af766bbcc2534510" },
                { "hr", "bb0fd5e517ebb64dc18bf891f6fcd5c1be5e15cfa953e230d8f154391c09bbb7e4cc201d711d376e73589d920245e01cef47fc1d678798b0fc29fecf7ea733c7" },
                { "hsb", "6406f9f1fb50ee0e030b7f7ad35b0210da7eb930b51f88e69a4b13f0a7cad5e059e8a4015959e3f21de107fa5606021ff139c44afdc1121985ace6ad6a4a404c" },
                { "hu", "c5bc268d255a8ea93493026a4237b01fb82465ccf9391fa5d140c915e6843c8f673d3e5d7f72c5a555b06319534b57e99a4cd6cce922f378f5724decee67d525" },
                { "hy-AM", "06f6d3c05667c9349e8989ab16cb6df9a1f6b4c289fc7fd776c57211243057a6aa72b607b469b9a08fb04cb47d2087f241bed16853536728d20431a870fa1aa3" },
                { "ia", "8d188b88d9c6f8b340e931cedf3ad3cba9702a5644a2dc058f030ad7bb6826dcf2a19026b7e48a46106c7739e478cca1fe1d2e6ccde7ca632e7dd40a2f3c38f3" },
                { "id", "648e360ca0ccd1a19b9268f5a94853f457dfcaac01ca7a3d03f7dac5a8bcd210147355578e456c8f0ca6223582159e72eaa88d2542b0827e64890e31fcfde5f7" },
                { "is", "3ff38a297d7cd268c84907a80bcc0d49babcf6213650105f497cca2ad0bd4224ba6743cfa21eb76fb0418e5fe12e9d76f0d7dead9215876a66b10890776e4f67" },
                { "it", "3b69285563f721006a67f81a1d26286f3e7c5b1dbc9a00fcc51dfa9c6ae2df2c9346c79f3a6dcbc6b99ecd1e928edec9bb8cbe779d3b6331fb1c4f76322bf9ca" },
                { "ja", "ec960443c750bc238202159b25db0b1f952bd6664b047cb98bc122adb10dcb41c565b7526f9e1a134fdd60fadb3bf625f42e51678836649f2345d50e45bc97ca" },
                { "ka", "777572357a59b0de157904e9cedb632bbb472c7bc104e08a441514b1d34d15895e6f0c20333c6fb3daa08ae532ef4cdbb2b3d1f1401583148d111d1ecf6a9500" },
                { "kab", "c4c0fc7a8ba38206b1bb51fde1b364d6887fc01b5c6a6421844be34c57fc39e6f8e007e567603b8337fffec0228d900b4637cded504c67a346f3237ffea83a20" },
                { "kk", "9356fa52782fe8469d88f6c6b888085cdc920856be5e0db04ae3c50a03b5e15b8a76e1da2753107cfa38c75c3507db05b7bfe7d0da54d609252f26a49df221cb" },
                { "km", "cca896809e31d89b1431d28ab5bdde72748c8ee598cffdee39854220d1492e094a92ed802a7bf447318cfe639faa71db41e61f69aab34defa1a175a2e4c2b4de" },
                { "kn", "59389a34f6a710480b504f4a304c584ecae26b2eaac1d920fafc719a0c783b6545a6fbaab74484faae3f093fa558373ea93a97cc84f339e01fcb36774886aa96" },
                { "ko", "11e1ab2398d1061df8055626281c41d38d69917b7cf9f5c45e3bdf2d8b08f80490e7f1272dd2f8e5a0da881b8a158c7ce483332265f2efa31c2fd255757dfc3c" },
                { "lij", "f97606af41a26ed63c3927ff4201876b31a0ba0ed384da81925938315df184aef25ce68b1e88294644d43e62d184b0e867f30fe8c8f439ed69f960afe9a12f4a" },
                { "lt", "f573ebe069c02d7657c14bb44774b1a19480f1003aa3464451543b4a4d2e1423bccf66f98574ee2b58d8d0271c29b3e4ec704ef871b04307c1ce12888410ae3e" },
                { "lv", "260af73a0458770dfea33388a477c698f8c13bc549aff7617955ffc1d1a49b5b99a765f2e0948e798723ba2dcfc50730f27c7599cf849ffb0e3d7f4558c5166e" },
                { "mk", "adba0645d12d53252a724a4d1c54684f1294e980936ead602b1d2410c290d2af1e11ec79037d64d286a969553d6cf0abac00697d457680bdd76dac50024ebc41" },
                { "mr", "76f88e82d600ca1da65e85534ad07285f51db0c573b3b115060dc38bc347dfb9e2b6f959f04ebae49ec27364c8766570adf877bc7e656dc81dc341a059e1bbf5" },
                { "ms", "365ed76320afa6d96c4863ed309fe7f2257b438741a44e39a09e6da845cfe00febf6cb3d68cebaa43890c0fcfe8e61c57d83266f330002190e8d9c88b0873a71" },
                { "my", "f4e892a87cf55b821715f1307c2db4bf32f9c95d6de60a36664dea1347145f5a4c2692817c952dba25c155ea6504e37cde5146d80fbcaac518fcd7d0f51ae8b3" },
                { "nb-NO", "c342b557f2779cff80178fc45fc92de5cecb1a5df1484235f175ac6c4b6ae9667cce2984656f0ea99e4212f78e8a4b75e1185c60dd178cad1cc409e9b06892fa" },
                { "ne-NP", "092ca1fd0750c1f64f27ffec36f4bf559280df8e3f35f3ebe675540059493cf0b67e67851021ce5ba27dd7f54df659ddcbfff7e8658916aa557f16a15e8d4f9b" },
                { "nl", "531a3b32fdfbde9941818489f80725a6ce168f7fd381595d81b592bf7c699bc2395e375075da10c5cf1a0968da4bbd72ab161e193956a094b6130e6a6d2e0f78" },
                { "nn-NO", "7e3a492857fc9d9c9b6f10b7a06f54145ee51910cf6c4ae9e5f0fc3a808a99e9847968ad7ad948f4bf6c4809f63f1dfa7f86cc455427290a915fe2ddbc93a09d" },
                { "oc", "b704f6fd61532dc797dfe41275637414feb5e00a1e9a6395d0d3d0211138b00d1b40e6db1dc5bed84b6390099f449fafd03a91a49d9b9d688f27a98585dc0556" },
                { "pa-IN", "a690d18a8767f76a51a7a569853cbcc2c448f3738270daf7aa3f2894e55c59064fb7515f2b3bd208c38090611f02997def20b62728a854603be71c9f3fc50362" },
                { "pl", "fcb6351c201bf4781cfa3445360a511de8732745064080a7da545af3267767aa49638c1ef48ec9bb5962b52444588f4a0a129be315067be4dd9e2252e0499418" },
                { "pt-BR", "8912e1562c160eec68ba1fe3bc186becff3164b432344b03cf9c00f591f95f9a8002b5301d84b89b70a5b113c5bf5bf5d863f1a53cb6e6882c02c244adea1f5b" },
                { "pt-PT", "56c1d7b04aeb26c665b1e4eaa79e0a41491874c8a855f3b5e19f9b90ae28d9d19f79a004ec3e8da44e594e45b3060d37d890e98e4085d8a36f40208d322a76ed" },
                { "rm", "fe9d86a9d3a11d1f198422f1d4b2c32378726327b78301d92c1ffdd87eefc35abbe7b47fe14f99709064f769696f1be5779c784c2e6e4aac3f5d12100a225a88" },
                { "ro", "39b7acb13d25e890a4b59fd1ca8ed60f90120547bea0a8bdc0df6ef58142f97733d5424fb4a0d815ad66137e55b5e6cef4bdbcb6068ac248bb3f69c3ff53c826" },
                { "ru", "a4ebe5ec0133b71ecfc529d0269459425ccde4dedd95eb1fcc046515b1075037504d54b3c49b034d8f20bbd5daf260c1b91243532c88f33c110c2fcf6596938b" },
                { "sc", "87dad0374d5c821cb50fe4a3ffcec87c0c1a9797ad3a61a03790333efd5518b2abc4d54a48b1845c89052d08573ec380b9904f9c7e5c36ea7f4029e5b26f69a2" },
                { "sco", "3401c06f4570b6ff7cd772441104d8119580e22feb24ed8cc3e1b22d9b7e523d01aa2d8de82f3f4fcbbef4cd65056a3da9a52038870a097fba84ff5bf84f6b40" },
                { "si", "b9302961524c8dbd3141e132081c5d9827e7092d36e368962e2f90efd6ead2db638d2770399cba58487017ef04b3e5d668e3b347aaf9aab213a35c6530d155bb" },
                { "sk", "d7b7576ef4c6ff513cca883e1287c4fb0cf801afae6b46f17d398f02129bc9d7d63c3935e3977acf6ad1e6534bd8f76b287973a9707a033800aa4637964fd9b3" },
                { "sl", "1aa060754988047361f3f31275019ee179b61e1fac8fd85f3e13e430c2dd0b4f3b51481b9afe9f39cafa1cd038636c5f4317ed405fbacbf12477504235c46004" },
                { "son", "2ad4c16115834ed26f7265c2c18bb3241d19db3e438a6c96cc987bcf16c85a46977583ff068ff3195585729310c6bd50e2449187de4a8fc5ff9a27513d86a898" },
                { "sq", "afd25703d1d271f80663d355e7a83d83085691651a7618feb0af27ad430f95c164da975c62b092c054d14e094d64483aa9306b0ee648d7b4f2a37a843a758393" },
                { "sr", "69217bed0f57f6cb42ea72e6a13334211ea093334c593d8469c372f192876528ad36400b47018509e205065f0aaea44431dec73cff774938a284976fe695361b" },
                { "sv-SE", "467571336f5b878594f686b95d14d09f08b8d1244dec8aca98daa3dfb3ddae314228f36afefe73ad6e2f43c373c1eb4d354e0567086a66836b121edac01de5f5" },
                { "szl", "0ab5d09c09c816e640af05edbb6f5944dbbe4a596a3e368c22768ff89d56bf3849bfc57c464679976785930ca33e161a3c2ae10b1ba1242eba8544f7ad010fa0" },
                { "ta", "f4991653ad6ff79afd7d7141d25e40cc79ab29044888d2bdd7c11f7641a5b10e26d1221ea04788702a2cc4e40ad2a7a84b46e5824759364e662439bfa86faea7" },
                { "te", "6601a14319470f5bf275db169b8d32a95376a6455862bc780604972d9125ab2ea8afb97647b58424ac96722bb0ff36f013ea9e85e2d9a32ab88e43281468ff37" },
                { "tg", "80363872569ec66053bdad457d0c40b715d9ec263d93ab14fae7cc9a7615fa128f285d873681819d10d721c6e60e979a1ac9df73e9d4f417fc47d768badc9978" },
                { "th", "efbd63d6385446d984e3da10ecf34fafed06644810948cdd5cb5e925bb3b8292592aab0549f645174973fcb208c52966d43e10a7fa16793c8ff29086954c2a4f" },
                { "tl", "85750730eb1911d14a91c9c52017ba37adeac31932d00b7adfb90dd10cc61bfc75f0b565e2547b07f708be69446641101f8db54f23a2ea107fbe15eb5ab5d667" },
                { "tr", "0f61fa22a6a4506f03c3af6a6008ae0a02142cc3ce3c076a0c052562de9b6cfb4ff4f75eb74127dbc914e5e67859f2d1fd8d9f048ea330c46d0be095d49ec3b8" },
                { "trs", "e8f1230b153b1adfc7db9f05d154fb76553fbb9140d54e4f1a2528723600fa69033529022ddcf4681162c72769214c280b5d7790c321562ce4b6aceb4f8c19e1" },
                { "uk", "7bc5456e27b79c4dc2a6b32ab2babd4c6017409300a8d09c32e09280af5df1b669d24d728245e1442f3b3cc66ecbf324af897ab6964cf6bd86005b84cb929f45" },
                { "ur", "c2d7018f123a1711f7fae0b8a72ba0f356b4fad3b75fd0f22adb26acb447d5192c31cf3f926d0ff99eed4b7b6ef0973a40b785eaa230f309b32da52d4d7f87d8" },
                { "uz", "abe3dbc3696130beea1652bef34e838d6713d2ed21f2848baa32c655d3f65cd6814edd63c52c5a309df51e89a4ecb07872b0a2d41d8aa4451bcad9113f37fc2c" },
                { "vi", "a1288c5bb89f0dab925a08da7b0f54d331ee55868eac7bcc0ecb2c6f4bd6bb9d1983e1ba8f86777fe0fa66eb8945297c6092d853f99bc1165198fc981519c9c8" },
                { "xh", "4ac7cff14343390b05b4089ea1af4820d2f0f19b2b8ecfca8016bdbee797fffc5ec0a919fcc5a7b2e90883b742c68d0df66e20692b8e6387ce65def069246887" },
                { "zh-CN", "32f21542c586c70d1f70743fd55d0f07ce975821250e0b31320fbc0e6f74786364665787379c01dd54a280fe11ceb8069f29fbe3c625bc976848c5e47ace6785" },
                { "zh-TW", "e243945bf1b096247e7102f0e2e59967d18033ff78f4faef3ef709138f87de4da27d523c525d9009eaffaeec7f451b3bf33fe5165979ae0e8d209124f81a230d" }
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
            const string knownVersion = "115.0";
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
