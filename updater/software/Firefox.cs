﻿/*
    This file is part of the updater command line interface.
    Copyright (C) 2017, 2018, 2020, 2021, 2022, 2023, 2024  Dirk Stolle

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
        private const string publisherX509 = "CN=Mozilla Corporation, OU=Firefox Engineering Operations, O=Mozilla Corporation, L=San Francisco, S=California, C=US";


        /// <summary>
        /// expiration date of certificate
        /// </summary>
        private static readonly DateTime certificateExpiration = new(2027, 6, 18, 23, 59, 59, DateTimeKind.Utc);


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
            // These are the checksums for Windows 32-bit installers from
            // https://ftp.mozilla.org/pub/firefox/releases/131.0.2/SHA512SUMS
            return new Dictionary<string, string>(102)
            {
                { "ach", "54ff4ec8652f4933b089c73cf13aa5dea3f31e00305165b5003ac685daba7a445350729d4560975a57ffac3485a6fb72e0c0d065a06673a4fd177b1df831349a" },
                { "af", "0c723f4408e7e50a16102f7567695e3214131a8f113dcef62a5c0ac31bb12c61d7146238789882fe414449bd0f319e92ea5a775a3b15b90d93fc80acfc33a5ec" },
                { "an", "737f6733af488f1381aa8e282e1be1b96e18cfa98c237842fe9e33efec29caa54d016e5b929a974b44d819f91ff410b0e2ca27a1037443c4e350d00f2c375610" },
                { "ar", "0c73b31d7a9a51ff41915b9127b935f73395441838954c62a9600cc1a4b08838d7cc66ccda70271600b954e70c7eb61f741338c24ce8ff6f72bb68d555e87390" },
                { "ast", "840437d8e27936fbecbe9acf6ce6536bc3c4c8cb05ed3852d2337ebe8f467d5ea1b3ac4fef91b053bade63cb51361f4cccb442a39fbe3f8f40994bdfb0508e38" },
                { "az", "9cd0baabc3701e6e5553373fc52ae1da6d66adec94451ef6dacc0fb5f2a6f280d9becdc2d6d9f7d81034bbb45bdbd87a5cc8ed38601d07d94d5d4f0308b21093" },
                { "be", "373464f4ba304da5ce8ff320d67d3fd2febc9637b3410d2538079f325a454a4d24d3b04f21b030c54080bf051dcbba7166386570e539251bc631b481f1535d84" },
                { "bg", "d1eab89a6a590f5e44515db0c32b30469acb9964964ac24778eaa1a8fff11934a096f1aacdd9a07c2d39ca98efeee545edcd533c873536c8d03b998eed60e194" },
                { "bn", "ae85accba904e86efd568eded9fd2b17de0fecebc8008f849743679adb318d2459ffce39177baf4e20ee1cc32de2bd4f80961af6a80d4f3b32f40a4fcbe434cc" },
                { "br", "0c2d34ba909df63fc25a93658ac37d6412bd5d10b656fbc41a1cee0252a89e18f8cdad2ae4887121fd5138b3a023eb4611c461fac5f565fa14a9e0bea22ce553" },
                { "bs", "f52d4032d69bdafd49b52ab3131a97346568eb8e3069236a25486e0600c47fd716ce42abd47132149bec1ce1bc8a356037b4ca2e1fc4eb39e1ff8252fda46ba7" },
                { "ca", "85bdf5f79d74284e4431ffe3a6ad144d6ea4c0019a015b0bc046b8d7232272ba72f3e9520be8d4fedb94720566d6d0bfa7ffe35452d8c92c77d47b2277655be0" },
                { "cak", "07f7531a77fd0c8106563abe66919a940b1d2525c862e6660404f5a8838565d8501a25b5195fde07b4a09002996bc891b096e6e15b89fd857d6e1a897c6c22d6" },
                { "cs", "33f6489835823e283248913885f534b45fd79b596e6d6837365a6c0dc295dfcf61dfb507ec04e35de9a3f895914a54d742f416d120122a49f6924b146cc98898" },
                { "cy", "aefbe40ba435b557b407f0e44972daf445bd3d900d48bee3daa9c34bfb049788c12f5b1fecf0e1f16748420af35a528fc9d6321c891afe6df65aa41335349525" },
                { "da", "1213040d48bcfb16b35b8a86e6f0ca31b5b4d5d2d022fe9f7ad56d34a3199ca74f33bb7baadf8a9ef5b9b386ab5e9c6ce9df6ef6416e63af8dcd2c76818231a8" },
                { "de", "2eefde41d05385e95fda8e3da05fc1275669a278f0da2cff004f6aa20723b8654df888ec9a716f246db4471c3ff5189704eac05ea804b9d015e2d782e375b675" },
                { "dsb", "2d85cd69d211517e2c0dd4e96818619996d64cb14d66edc59376248f82de1d6a41ab81ad950d8fdcfda55b7ff79f19e98de7310d65eb1b88e0c317843537b351" },
                { "el", "77de610f8b68e650ca768795d4a664f948d5f440a419755cff69303706136d1c893f85fa3b9fec3149c44e71759d77375ba5021954def128f9a64acb74888c2f" },
                { "en-CA", "85ca603937ad0459e289b1eb62cc82039cf465e971fadc4cc88b7dff77f214a62bb2fc6f5387415ecc536ebca9475f856bdc77d2dc98adcfab1755ce938e520f" },
                { "en-GB", "da7b7a05e63f952994c8adfc37b9b733131a77f127644c486d6e026ebd78036b6ea09a758ff0b81e7e81fa265c23b358293164d9809545430caf96d79c37a8b7" },
                { "en-US", "09f11e0f8ffcd3bcfaafe8c5148d5296fe589dbd2ec7dde1cf349010cc2c05dcfad919d14fcff4d3cecc0bb5f6b208e46b424089d84ed9ab3c183b3f44cf9ed0" },
                { "eo", "f7e10eac264c82028771836d97dc0a768b7e850c96a9ac356a0b0ea9844f9dd6c88efa462a86af2fdec56bbd3622b28d7bee4e4f55c655170a5f4d4ab92617af" },
                { "es-AR", "81c02aea61ce238ab97cf2d9715fd2322788bc4eb8811015f2d760a19512f8f2794302f2cf830391911423406a18d9c7e74c8261a97f47257a12ed6087b2197d" },
                { "es-CL", "7f6154e2d8779b0fd8acc43468ad1dab4edba56ac3e477880852105c701d519a5eb325de2596ede6bb7d886907a500711081aa6ea573d4e039a92554f5ec2631" },
                { "es-ES", "0c588e22efa79057b39615d02d02ec4360f789cb552d12008305dd4e45595dd5cc475e292cc55db7f03939c30b1a11e4901143a3eae91b06bad3583ce93fd8fb" },
                { "es-MX", "3d98e86906284ab397d9ac3da4aa8abfbed28a3c0d07f505aa26b88a7be183448b9c430930d18b62b07fa96c0a2e2abb7605d1da86a3536bb56004091dd344eb" },
                { "et", "7b7d23dd6bb13bc7270beafb222b2e65d9b727f180140511f03180b5f780ea5baa743eae1f9b5386cf093ca5b9b6b3969df1d1a6fdbbc8369dd5e7460b1d22ec" },
                { "eu", "91c3902664d559dffd85d6b5dd6919641cbd52338f1190d44042336c4ced8142a3facfca31c31b9fa262b636554c6d21d946ddb4fe9342133268abc349aee0f0" },
                { "fa", "30366b01564b1a0fa03325f84c29b3045626ee417e87e6e2c72943f50e05b4624358c23f9286aed5ff521dd6ddf2bb23e539999f2a73349c59ba6777170f330b" },
                { "ff", "72548732f8d460968d6f4115d490739b7beee7d11008abf3e1c42121ba537dc0ac1f31a8c773e1d41d7b5b5f87012b6936fadc4ec0d425b6b34b8bfa3752c7c0" },
                { "fi", "2cc8e1d61dba82499c04aaa486e10d15c33012b055ac9b2679249db389f8f9e7cf6bfa609d039febb73f22ae01ef057e65458ca359ffa363d8c6bda1c68dbcca" },
                { "fr", "763f15b7cc77abce957cbcb82cbf16232363355f5b01576cd18e12ca76f23e2f572c5dcacf1b55bbe07420f93201da69855568fe086b6172cfa7db93bbd32c66" },
                { "fur", "b0571948f3e16ce51b2c851754a3577b9bf040da24dc67425795134fe88eda17ef36cdb849c43bd055af31b04a9a1e30003301344007e9ea1251be2d2ed48c46" },
                { "fy-NL", "770cb2b327737a1e570dec4a4fb16f354ebd47204eb30a3eee364f7441c67a467ea5669aacd33ca56895cb0e0264f22b37385c0c9af5e69386c1d3e163e9ffe3" },
                { "ga-IE", "1d96e78ab594084713d48645737fc68268cf659bc5fac6cd2aa8ddaeb59236280d6085c07e441f381d44134ee9cdd14811845d9a9d05d720cac19e97d0fb4722" },
                { "gd", "8386760397d104124ca25097ee44abc1959271de4e2fc5373a937ab1bc5546d4bec3f95466e502f9653f2a7559b5f901d75a7d4c4f63e1bda22906b693b90d1b" },
                { "gl", "95bc8c2f9da527063b6e4138f5207c7f89d06ae3f648004d55b164deae17498cb5ed1fb934e7c7902d84aeb9942ded45227508034b28debde94ac0f99c7a2760" },
                { "gn", "6ba1f1df1ccfc02df5d284f8b5a7f1f6b77335ae815050cf34555105b12880bd64114b9840e77845e76b10add3528dffe99432f5ec53dea22b9c364e3d042153" },
                { "gu-IN", "5b0c9af55bc28a998011b3c47a5d08eaff602aafd042d871b7e4a8d14ee2efbb35ada1075287cfcb4e3b10b7558050f0b11464c2a406d4362f05ae30212375d2" },
                { "he", "a45a00bf123a7c2114f502eb787862551b8d815a66dddffa32a16e77992475c93c76604af4ff909600ecbfc0f90e0adeb241f6f3c94eabad0f394488e6783316" },
                { "hi-IN", "23576c2feec5058b663f913eae6c1a825c60bfb159f6893c6e33375f698aa817a89788c881e7a49374c5659155225052a14ad3a96d6fe55be545714d0fe5a3a6" },
                { "hr", "1b0914f1acccc514b1bba920fb4125443563b058e480536a5aa31a04ce5b1348956262d94f87fac46a024efc11258ea54d832c63b94c38dbd13eff8eeac1f3fb" },
                { "hsb", "1c3554e55996663536cbf0f7ae19e82c3c9fdf160277878fe6de45a7592c8d24dd76216216c28901f9be1a7f65153b9eb969a49d658c581710e5a50055fedf71" },
                { "hu", "c22c8c47f094487f0a4f0b32ce8092b4067da019f8d824f03a66a7553e7ab705b5fc695133a9713cbe573cff270efd4422ba3b9f2915edd7473e5016f5c7b4c8" },
                { "hy-AM", "effd01fd941273201615780752672eca5e768fe162a6026108c84e86ec007d79b5fce6681e312fcc81a631927057bac6d9b6f939dcdb1434a5517552c5203e85" },
                { "ia", "5a8a4b7d65800ed7796b4bf2a9e2d0271454c660747dffbd263894a91dffda5ca87716a0861dd93473499ab3fb37198d377b1a2853cf8deaba65757333289be8" },
                { "id", "09798e59787a10b3e504025383f447ce1c3beacee1444b6294bd97aeb7199f6640972193583bfd5a5231fd30fe28557ea6edac90c07779b1cb1d651be2c30dc4" },
                { "is", "422817177e8601b7d3f3f946370ee6102da70551a880b29ebd90675d29cab00944783167800a870498d8f628e140eafe4d810b3adaabce18d42770f6de10f485" },
                { "it", "a9fb2a89d590f5d0a37fb5f7606f9fa8f5a01b1a9debfbf1ebed3ab373b35fb4da85cc97b8b2fe0627fe96661aa253edbc265b349d2e941fb97a0f20a8bd9ad0" },
                { "ja", "ccdb7dd3c7694183ba06548d4ef62770a83d7e8ce03bcbd1d4c6d9093dd5848b1e0a70477ec6f6fc90d6be2648cce08899391cfcf2bb789925d429c7acd4c1db" },
                { "ka", "79aa3ec95e43cffec740d87e52339b578b3e30e8a0a44ff638ee90894e413fa4f74880102dfc2c7b1ced047cffa98a9e2b394c5c90b9e9b5719b26e9c11fd27a" },
                { "kab", "7d0b4106bf23c4b0a6f5a854e636af4e190baa1a034f43d2867d601156b635efea85d181f4e72f8f07ffaa72d9b81e00510d32688152c4506efd0368b8747244" },
                { "kk", "7f5ef0f468873fbc2e50bd18f0d84607cb1f7432b33f8d5fa693e9aee57e93fe22abb6b71a931cfe3101ed051f30ea2b1a6f646f34718825c9526fd1100e7b17" },
                { "km", "0f2341cdd6e3acfaacc49d5545dafe0f43db0868f2f1cbd89b93ab8756351662a473a4db4e1f24124606d069218246a0d9810349294426bbfc6079dfdabd37cb" },
                { "kn", "227bb5b3df55a6a885e0f1934e2f75e1538b7cf670991ad59512a0ac1a99920dd1766dbe30449da3d1ff6eb6d9254fa1a74a06078aa99636dc6ffec01c05d770" },
                { "ko", "f5f5fcf82f132c3cc820deacac40b0942b81905d8c1ff16000d1eb66aeb7d28d9b470f730ffcaa07834087139700581d8336489981e3cda1884d99cb91ca905f" },
                { "lij", "4fbe3e608fd12a7919ee7745f2a7db84cb15dd03484237f3320a29b5c5cbcaa9beb8ecb70bf8fdad6b9ad29a8634db6cb0bdcc330929fa550ec5155e6c7f4728" },
                { "lt", "1898a5a504ddd40f573585ba401c2283c98269a5fa8585422409c25858d8200c2154dd3d147c1815e3e60beb0daf4ee08313c4f08f63bfde19d9acffd4a0153b" },
                { "lv", "924921546f416765019ada66c74076c8ef28ab52624a2fbe0685d4a62856c61a804cec5cb48584e11c27fb3d6593659229c3fa66b84853a469ea268d425e48ca" },
                { "mk", "05dae4df31ed7aafc83a147cc73061f75527af6fbcde4ea2b2b5e08377eb2fc7c2d3bdd43f86d15441139cd921e06de779aa07d6d459c1c4d718c361f06c2787" },
                { "mr", "39f6a472f9f4f6a05c79a1b756d8ad442c0226d59a9dd39198d2c78396b69a22daacee8ec6ce8c5dffbb9092504b027d593cc2575b02a03f3f45ce90156530a7" },
                { "ms", "32f370a91b3848838e5950e1a0cf5312dce685a908e7b72569ac62b8ed62bedc203c89b646b937e34bd771068df58e1ed8286f8cc9d9be3ab7f864144390898f" },
                { "my", "9031e6264eedc22b468156f69b561e42d710ce8191f69b2096e88bbb8507be218dbce335602b920d4a66a604be4ab2568760f9b3a6bda1fd54471ab9bf2ecea8" },
                { "nb-NO", "df21ae7b38cf5b195e6794b816233e76522840a5f78b5d91bb4520a0be7da27845feb2ba6d836eca47962796a2812dba9dc78ac372a321f6a1aab0900e43a1a7" },
                { "ne-NP", "7da9a60701a63352776846a8425904aa3b9ffe19a5c7c1c27eec1b95f8946b1fc85b06ea74f7c5c838753e707052df36fa247dce55a82582cf132788c27a124b" },
                { "nl", "c6ba5927501a00e1d4f9e245b662bd60dfbc8a8d46c3f320d0257cfb317825cbc90edc99545d993b19164cf1b55dd82df923ca4f64e4aee216a1dd195404cf5a" },
                { "nn-NO", "79a53fc3f005e890586907983d53b6a4467ca688e051248272ea64f5583ceb85a2b136c6c67c2269116402dfc985a5fee8ed2c93fcebb39734610693e6c86fc1" },
                { "oc", "cd412edb19ce23386b665e2d741c96a3225491486442716cd254298564c621aaf268cd7121f5f0bc23c13e4fd79028fb266f9d363e7003e17f6375c57c8f38d8" },
                { "pa-IN", "7eec1f91b7702ea487de918313e0a5ad0b3e6cec8bfd00690987ca79fff1fdc93a3d655444648c3f9fcce4000d22e67045a45667babccdb71a4d95a272f2131f" },
                { "pl", "d2aad4136bcabe42ed799386319a62f9a03fbcc18ccfbd34e69622346b7e58e3cfb98de65da65dbb00b58891e0d79de16785da518414853da79fead1ba4ccb96" },
                { "pt-BR", "dc2008581329c34412873e7711de8649f0bec87f2166a2524dfa974b98767edf19feee167de08b4904721fd749e2f2afbd059d61ea33a27d4ada04e7fa35b905" },
                { "pt-PT", "22c12b077f32a6d2a3a18c4b6832f3be288748dae1ecc42f701c00a2ae09d5361e43ed4fc0cfa2737f69768bb4aa1b42e8b100c8181760f9f035703d05d0b5c9" },
                { "rm", "67fdf9d40314c2c8bb4240457bea6a40475c6bd534fcbf6338d85d2af49986e36a3dc0eb67a131aa443f6a3e2875b81abff3d1b9e1f85cd6f798beba4eadc862" },
                { "ro", "f06e6a1f77e940855f9315866cd0a58e7eba2cedaf6518be4dd11ebf608f877915528e8ea5e35099c1e20a85abcd53307338d6274ff4ab6ec39f9892e6aed657" },
                { "ru", "2ad9d336133628d1e4520bf4c3cc82b2f2cd418fd95c961d5e7fe9862cf31283fcd394ee30694f1eb19c87b5414cb66556cf7f24dae9a01f57767aca2e1f004c" },
                { "sat", "0fac943b2eac9a2c8a2b74fb81a44cddec11e969ad6318b996671f373cb53debd17e9b9b6df66e9d4d2e27b8e91a7fb1971bd13bc6ed7f52ece9ce4152b9ecf7" },
                { "sc", "0d1f2f0242ea9865be49dddc1152aa3cff9e8ab47563be11bec65b0d2b1d04e2e0c6be2ae238b8a950cee017d5a159ca6efefea4e1c8451535f47e945d6cf3ce" },
                { "sco", "630a7e0b1a95d00ae749ce52b4193d35d296500e290a868a27aace22f2cbd34d876122fd388853184d1cebe3626c4a04e3abfe371e56bcae6b3beddd0738939d" },
                { "si", "6b611f56d4e754a58e59f4ffa9f58778dbf2173026720539ce7707cc29250d1933929cdc336bded80da39dcfa798a2def6ac1c65e7644b68e62399e245a0f617" },
                { "sk", "6873ff629b26bfeef5647e8e0ea17e79333a8773f7dc822b7467fca2bffb2431e5d650fb0eab499d4f96d49f6401adeac8816832c6dedcf626bc9a7d689231d5" },
                { "skr", "c7eb7334d221b4094a364f098dfa03a3fa8d9fdfd6835bee0cd9e081a0e0a929b958260413222f733c4c64e50d03a94fd8d577901898b14cc9a541fee3ac63bf" },
                { "sl", "116479781a989acaeafd519c35f14de38d259a60c48b95e1045c0e772054aa932aa948d53e34047bbc3c2195490c196f63c1c09c36bfdb1098ee807be5877d34" },
                { "son", "da391014fd627a07fe8ddda058a29314e6d24669638db005b6095557a66e72c970c42ccb80c0c5432e8c94a71f76f6e889fdad4ce2c6cd51e492c7b24569c5e3" },
                { "sq", "9f5fc4044a0daa05a8ab64b6d2356b61b4aaf0e4290d528d8b516a4c2e2a8d9d3e37e445c40e2278fdc85066e2f738e3484a17568bb8c0b6d4180a46a19eed43" },
                { "sr", "57f5189ef7f00e150c3ec4a2f66da743280c4c5945b655497a57f77ff2432b9e299a5a2fc73bd8de49c8971ac41ce150619b7098a6cc17c51857138cb2ed6e12" },
                { "sv-SE", "4167ac5a345dc3da0d969da09a5852ed27762b3038f6ae94b4ae14d5157242fcc2595d08d4096da6e10702b52af390a313235084eed5af653340900ea0439c9a" },
                { "szl", "0ce26fe1690a10d5be40463f6831405375f82c7034e273471c0d8df09dfacfce7e041172ac49dadecf348c3411c57e9b583e1139da01e0dec9100c27097b421f" },
                { "ta", "da40b7015736ae0e1d27964626e88036ddd79ed3bddab2108bb0362ee375a43c862f6b81f54774b56d84f544425715b417430d67cdfaff7530cb67ff308e0f2a" },
                { "te", "c62b0c71bcb2189da3445e66a766f40b235f6694b449040b91f2481deb8b2d3eb04eb9dfcd26e78d23c5613407bc610c10bdff293f60348991bf04a84824df8b" },
                { "tg", "343cd80d692de8c2533c92dc86305fa4a13a1aa4c8828c086a4c79f90ce4ec4557aac5420ed328380859cb7b2cffc7f19fe60c968bedf2dd5f8c672515e5a292" },
                { "th", "4b133b1b6938678e465ecbaa7f65c573de4633c37e6958dcbe282ea5e1265899a8503bcc939233199ed94a9d2cbb265d3c5f1c36d7df0abd58eebd21564229d0" },
                { "tl", "66b01616986871444e8196042f4033ef1995555d0c7712e743f7839fe76007cc552958e382c392e1a08d08fcdcb7778e8ea76f26c788326f4ca35d8b7353b8ab" },
                { "tr", "08857b7f5e881d89f80c47caa6f15e8991fb81c742610972d9561e386bb8a2fb16e5eb131ceab10c4597df77b7f4f8ae76ddb0b76637362f954b3d9f7def26ba" },
                { "trs", "9772c0ede22b897b5f920eeb04def46dcc3b7fd17bd604ef6a38d6087b68a14e637145855471ff507cd717a275823117927f21b06bd560c79850db75a8dec064" },
                { "uk", "e876f7f8e3007f64dc3c63aad9f5e28ef0bc5bbb13cb8ab642a10a1ba17cd6cccd0943e22852ed9dbb971130879a2ffde7a08181e2df8007aaa062b14df9c65e" },
                { "ur", "56b88d21bb830faaecd2f644850e893ac841bfe27e1d9412db4bb7d2dda867cf817b04f91298c58633e7cf684aff97ebbe1ebe31de818f00ede2e32f1498e02f" },
                { "uz", "4ab376500d9caf33da8622fe8ad0244b87e70967908a5985bca4d68c3768e5bb3d237b86cc021de909f8d9edc20ded1239169830929b8ab197f5b2fa0584f9fb" },
                { "vi", "447ce129b6bce9ad191c9af7165b9807b86bb4cf7f64bb0bddafb8bc3282140eeea7dc39358ddea054dea4bc25850552f136bc7af935db8f47cafd5cf3f0a4f3" },
                { "xh", "4969a360ecf9aa86649a90a441056be14f6a22fe09bdb69132a5a227dfb0e0952e4e4b9f18143c15824f54392109f8eb14efe63febde0afa58373d23874ce435" },
                { "zh-CN", "c8d3867e1ea44f20a140422319533af3c68bd3e402eb8fe4b9ad09f61b6e19eeb1fb74b5b4d5ebd8d0c3f8e5601e23431d6b031ac7d70876d0fb832be86f755c" },
                { "zh-TW", "c6961c5bf1b49392099039a038758ce5ed4c2196c12b847e80e3806b726dfcf2f44c986d56425b3b2702aea223640d05c07b5626297e3de8a260475da717b6aa" }
            };
        }


        /// <summary>
        /// Gets a dictionary with the known checksums for the installers (key: language, value: checksum).
        /// </summary>
        /// <returns>Returns a dictionary where keys are the language codes and values are the associated checksums.</returns>
        private static Dictionary<string, string> knownChecksums64Bit()
        {
            // These are the checksums for Windows 64-bit installers from
            // https://ftp.mozilla.org/pub/firefox/releases/131.0.2/SHA512SUMS
            return new Dictionary<string, string>(102)
            {
                { "ach", "15cd18071bfef8dda3dd17569176706039eedc63cd6b7eb4a11fdde3573423c8996b8cbf2f0b6302a3ea49cd131a8571d8c0b88e38edba53d9a939c126eb9fcf" },
                { "af", "f989d3ce5908e650463b0b0e20a3bdfc4e1f74a65ddd0e56847588b4e2936e0bdea1a9603589bf291c5322283c0a054da3349352d1536229adb71f0896e5f470" },
                { "an", "cf1297fba7b9403bb8a2c84e3f9c557e3fda055952e5db2e293f0851148fd05a18e231ff8913b5b1c20dd66d498d39b6966762e210c7ff7ef9e65d71af4246c7" },
                { "ar", "d80a8b08f39f3e64f9f18b244e8cb80aedef6ba1d7e9df53f69a035998b7dba04991d9559ad1d60383a39180abb860e680b7d1a0dc74c7ff156e42606c22d91e" },
                { "ast", "37d0ffd3fae2615d5711d92e2918be08f210bbb91830fa84af0f1a70965d0f9dad8e536cb98fc9719f5150168727549a9769e2bd2b9b564921ed5f871f692171" },
                { "az", "16310a0d656e9f233015fd5439e82a6b0991b0d31f96c97f10068059e54790694007e253fd72b78c15744866b188084d70703e5186539419e75c569c192d827c" },
                { "be", "653a262dd47ed33d580810ad7be1138e97b732aab01b14781f4e14d280c019d6a026e1aebad3dacabfbaf160ddb6338e19cc1fda6061b987e3d080614a023960" },
                { "bg", "58974f8d6d96871bce71207cabdff1ede56d9b060f38ac68e095881e8a1c4e7b0e63e76720623f44c6d87d3d1c976d0668cee2c7debbf7bf05f3ffe004488657" },
                { "bn", "fb40944e9ea005c0cb9bf30a7f7c84b3e3035ea0c6341644e2260f242f1bb899ca959af787b2a8b8e587e1a6765d97a2f14fa664b868e97ec3925d2a30156eaf" },
                { "br", "d51d618d0362b32ed73a9b896f1b28e2c68048a2a842cee86b65dd280bb121a28283085fa3b34b5934ebd29b04ce34a54ac14c3e7e5a7598af6e2438d07f577f" },
                { "bs", "d67cee5d60b7b7f46c5ce8252f8386f169b03d8c1c283a80dcff137dacfec7f82512207244097992a128ae2443fdaf987522e332e633567a1f1b0447091817c7" },
                { "ca", "efce9bce2fd4ab541fe7e58944e8dbe9356c44e98b232d7b9fc8a826de03a4ed2ec8c5d6b953c2ef154b65fb6c8028aeed3a9691a9a1bde8878ac109a8e3e1e6" },
                { "cak", "193c129e0734b1d7a2d85c4ffe0a240182b7c376d3ba839d041e2b28a0bd503c75841ef5524d03cdcb53528e3d661aa355f0bbf021df39c8d4b3f4183f075a78" },
                { "cs", "59ee1815ddd70e9f3f74cc830c3659a7e93dce3828240f95e3b15fb0324af3972904251730d4aaac9bdd5c7ec459d2864148181043ed1405ebbcc1c5c7243094" },
                { "cy", "a2beb6a5c36626790c4e838a741396b7e3725cce0f87c73b21d5db73327578a7ebc387090fe9017f7e4731da21e0b242e177248c62be061e2f8645b4c897a5b2" },
                { "da", "628a26e73a5985dd79b43759c0acb035ed945a148b833f2f30228864ea23f48f900be0f0bd1ca2c8141c8bcb145f2a52021bc35fcbeb0435a092f89c21a62871" },
                { "de", "690a03cc0509e9701d4a8e1901fcc6fae18e83f379e8807b3abbfc065a99712420f9200e30426ff2ba3fe88d5c96e86f5c3d7c5ce287ddf59577e293364aedf4" },
                { "dsb", "0f721d7805bc1ddef2b4d6fb49ae63f978b0813e151ede80a57e724f02bb46d33f84933fbafa14043b33c34cf56b3b569a950b966663184debe5095efc3a0625" },
                { "el", "95d58af067d8f16b6a5d337abe6e14b79ccf81600e990ce9e20ed6b265149b853371f9fb3c220553f4aff97afb8fa4e14b8edbf1b701ab1cccfe5b08ef963784" },
                { "en-CA", "0520b6768a5b5a67dd01fd94dc8d0e3171c7b4cb0e70027e0170c3600a133b95dbae02215a03d3531304c514127ac8e515b104c8fb9741502fec9ff6ca3d3f83" },
                { "en-GB", "f0f2d09c305de13408e247d27b87878388a4571e27d9416dfc9eea7b0de47739b169c26e8436d44790625450c0a28e720b54fa76aa4ab33a2d2d197aac72caa7" },
                { "en-US", "00cdc789933412fa897bec110bad8890ced74ea635b3bd7157dd1036dad790bba1cfd7ba6630a1f2ad2b3dc28774ac9a72daff3cc67b16c8b71a740ec216d6b3" },
                { "eo", "d37215854a77ecef7643cf35410212395b13796bf2b283ab51db3c5cfef9d8a3a744118e25f880d91978fd693c992b8edb08f889dbfdc92103180a45cb521dca" },
                { "es-AR", "a3c5c203f70b233fd5dfaf79a2d113b8eb334072bd2a2e0aafc05917e4f51cd218df18685340d6c1f96cc31dc4d838e5515c453659bd307648699d9a614a4a8d" },
                { "es-CL", "69d2dc28a929cf629f4d4aa34780f911a3fcf1f38235e7c580eda069c53a43fe7ac043eeb215a5284bda62deac3144068ca3a13639c0a76e7857495ef576af8e" },
                { "es-ES", "10463b718c6546570f07af2959fae02ce70a7f9deac0beec1a567260ced56d8cb8c7dbdb3428879150d1ad9b1829b73713b3c45ac9febdd50f32ef0ba33b0fe1" },
                { "es-MX", "9c5ca3acfa5537941fb6f10f352b237bf0486dcf33516b2b4693659da7a61aebb65c0c958de2a501bb34663c6a7a901d4b56b17250e07165e546cef7e951b596" },
                { "et", "6c052c9fd3656e6b015c94c388e7c5107dcb02d5d289804d87f4b29368e21c67b7644cb38c1af1ac2664becc518e95a0113758e40019e25f4b9c1dcc404ec908" },
                { "eu", "45cedd4626a848756d94ed4390f008eefe828edb155e40e7c34b347b1cb4740f65d607f17c29f332b51e9519ac450744570aabb44a2dea88cd82d5fe65b88e21" },
                { "fa", "fc2c40c090443d80b730976293b49317ac69478b0ef540cb4595d8ef0cf6b021f00a18bba13f137952e06df0b5a029c56692088c1d3fb1ac7aae0109c82379c2" },
                { "ff", "a63b1754342c129bb6f471497443ef6e82983da12115422b6de9ee0e751b7bf9b57980d4eba8a9f41153a44df9240f5093587a47e71c4e0dfafd738e71df55b1" },
                { "fi", "31d0044574f32fcdc1d088fc364d511f755b05158152f124cb6075b5b1ca2bb2c120beeadb420177db4e265e395d0e053b8f5fc7a28c6963be2e1554376359bc" },
                { "fr", "3ccbdf1eb440b82c45a9637a9f9a96c13100255034229eae7becd6addd40f2fee11fad0cce252893e4f130cec9f2abb2f41262f6af38be3644f498d2e63b871c" },
                { "fur", "a3f9089584276df47a1cf9e129bca7dc81f1782a6f1f876aea6ade36db0fe0cf7ae82c249e79cbc296d66b1bfcd6f7cb02b85a2bcf83fe56aff5e1a7c7fe2397" },
                { "fy-NL", "21a4bde3d006e5059d0c6abc6aae9cdfd1b47762f35f755e8c2b996510b8ef86255caffd6d33bb31245e86bb9f1817895a4feef366732b8064ccfb157731ea2a" },
                { "ga-IE", "6e646bd76c504927c1f032f23c8707edab2e0994a93853c0c30ad23636a13dbd73b939b88df7243a12789f5165db81ee034af4e1fa20735c9e849f7b7c2911e9" },
                { "gd", "9bea33f7a24d5b8d3e80e5a534e9a3677481601053371f5f5eee64e29aa936a833043324a13a37d31417a6bdfa799523f7624f25f8460d67f527fb277ee27706" },
                { "gl", "a7227827c558a1b1fb6bd1f3bfea2c1cf3aa13d434593857c184c224a20fdbef03eb8375f664c9c8e74ea41c1a890522b6054310773a50df66724cc00a4dfae8" },
                { "gn", "72a8a1dbcab1c46ed7caeafda634b062e4bec1dae0ca27032b4112e37620de95b1019904913a4c1a68f21ba3f4e78ec3f3e6b33c6f3283ad8f361f0bd627a950" },
                { "gu-IN", "6fca6a1f2b7d169eca19b43f9d256874d02f9d720335ea1722129a324b222688856c26fd996008ab61f76d00ae9eccccf54df5ac8e6b547e359427eab915d886" },
                { "he", "3b3b4a10032c2ea8b3cdc3b13861d92cc1034244525abf3dca26253fc8f878c4afb77b68cfa177a979a7368dec22ac868d0aedad9c68b12a5d6da06f8e5f6bd2" },
                { "hi-IN", "193fc7c26c5551471d134db3345e81b6506d10c25b6d979347f1266c2772fef664c1ffe1c22a3377b6d091cdf342c6ada52536644cb5e8e3f89af8805eb0c33f" },
                { "hr", "5994d9d12706d4053396a1ed2a52957d6d3b1db396340546820cd6f4b7ec565ed00839e5c6f453c25b26ffb657e8453791c5ba8946a32f0eaf658209661f5736" },
                { "hsb", "8e79915c28d1606961d2492aee62325e295c76ae14d930df2dc3cbe509ecf44eef8a73ee640c444e15f51dac8b93b1a98eff190a6946094bceb159752a6b20ac" },
                { "hu", "1ba18bca7420830129da604ab4dd23e37a3ced0f1db6e663853c64599ba4d74072a8690bda63216ccd9ee16d863152cde4c0af61cad5dcfc42a5d3bcb7d56e17" },
                { "hy-AM", "750b40d396414faebfe3bbdd1e0a8bcf9664614927346f1f2ee9b36019b8370901ebaa2ca895cb326be971f666583b833d6354438fb1575e9bf974a2221b39b8" },
                { "ia", "00cdb1fbed1c3584a676cd7aa392ea84d548cf8b3204bc425ec94d2aa7ee034c5edfbc0de0622d06b14823c9970a8554fc39c2a25c53ef5bf8a1c5ee058d4717" },
                { "id", "4a6b77649650b1eeb932ec9abdb00aeb6519abcd86b9ffd5f29aeb363753066f12ce45131c5c201813cde570946e246f54cbb4e1a9e449921221133ac0d717e0" },
                { "is", "0fe1d12133c6a5a2db907b39bb7a3733ddf3063b0207f92962c2f5b1d6b0682a84f600c319e838e1bf112845d9df335c9327779c0e8f907b2b2eb2b0354d1c84" },
                { "it", "581c2b26be270e04bdad65b271dbd54184ea6e7856c55668d3a8d3681e389c5c8cbdef84d201ac9abb8a12c685eee1885864e6c75a516c535a16d69825b700ea" },
                { "ja", "761a46353249287eba89b00c807bd3bf7429eb06502432c1cc7e4beb737902be30e50ea06eb50eec726d6c92678e13b6fb02c94c96a37a72e49ff2d8da56580a" },
                { "ka", "c1d0a3dd6bac0fa4080212b61f5fa13beb567f06913f4eb0688109e8988e92ab3e2515048e8340eeaf0a81155246102250021dbf0af440ce1282daa3aac237fa" },
                { "kab", "7addc3feeb30f7306d992003f38f085cfb603012f6ef75e99ca246ef0a0061b83042ff2454958a6eb703202ad7e8f8ad66f68b1cf74b93fb37c27fda76e80e91" },
                { "kk", "d7b772d315a860f42e8f59e2c22249fdc1391e949938faf490bd1199ccbebc0b5dcbd8f0b648aefcda49c70f960caaa8f7f3d29ec42ba8051ec91ed850f9ddc3" },
                { "km", "b460847ded0facc3e32e272f9d276d24d64b28f35e2f7488a0cf833f5ef0e78838fab0d62b5b158f98665a27e1da2d71e7dd41b3dfb4b58c860f1749a03a33c0" },
                { "kn", "f7bdf0d15189cd62ff03978da25dfd52d66cbd976775dec9b224ed51a51649ee57503f7d8d8c7b27ed2c81c562b4bf64307ca50007d1910099a321a40adc13e3" },
                { "ko", "9420e3d99168b716a899fdfeaca3016ad3e2fcffafc07a66ed7e668632764f63314f7c5e625decbd767f6e235cd5009b715a7b0cb133bd912ec5b1ad7af35e64" },
                { "lij", "bb04e85bc0aa062337ebbca0d979167078f082f843ec4f3af33ffdd06d2735c4e6b859a72d648d894e1358ace5577bbfd838cb70bd950cdcd47916384fcdb556" },
                { "lt", "846db233dfc2df3714cc5c244650042834f4099e1b630edcfc15782eb11f1b44724b1da3b6562f0604ebf850c1230cfb0c014991f3a4d71cc4df95c090c46041" },
                { "lv", "95ed323d50605e032d79405db75360db1928a509cc70223a0a5f88d3bd8c5d3799aa37d95f5db8392c4e1f7b2a8de394f7d01cd76c5fdc69a01c80ec782839dc" },
                { "mk", "a81fbb09520e7133983c07209ae6f8371e813fbe42db6e97d38ef798b0cc45b2765b3d347fb3dfd1f4b274ba37da63607668062939c7830360a05e1877e8cd59" },
                { "mr", "b5441401b162d65ac73e0afca77c76df30d5e776204cf9b9e4c27d298c6b8df09f9e4c1fef2435297677a5a837acdfc5d9a72550554d16d712f67465953c782a" },
                { "ms", "98a2e27aa97ccc46d8ca0781c19cae29772b4ab6e83514d7593ca179d62c3c2c7b6dd701508cc8e38977d4a7f92978b092aeb75fa86de2a70d3a6a94426be9ef" },
                { "my", "c2d4063f43e7729066ec14f5ada65ca0da4d6603ccfb957c3d9a3566bc49fb45ade70c5c1ffc003c7a2fef92a2424bd864716335b97ded0e2be48416f5458b22" },
                { "nb-NO", "7d9d258fcf394b8ecb4dc3f53f25f849eb3beb301c49d268c3391ce9f7bc523f91468ad4729d5a8cd53a2f15609be4f78b9d3e89a42e7250b59e42c03a8be31e" },
                { "ne-NP", "765336c52e21ab144f1c0988702a6673b5fddd368abd00ded2f18b44c01f96e7f6bd3e8730a2e760f05cf27ed3dc6c327618753954f5250cc4f1aba086b606e8" },
                { "nl", "f50ef05c58624bb6e3b7b2a064757519d0b6d6a7a6a34dab787724f5e1e6b5383e6f997f4467805ade8d179c5111da3ee895b21ca8051eb2c85ededdbb4c930e" },
                { "nn-NO", "5c07a44b99920ba752e2d8dc85d845adb883aba562a851c61902cfe99decd84712730023afd70d9f378364f2e2b0d3a441c13d9aab0484d1a5f63e970aebfaf5" },
                { "oc", "5b06a1c5cd0e01bdcc630f5b1aeaa777599bee8c9fa1518479ca65a45b8e7e921f41222107fcc0f5fe7e15ed6f83ff98ce5d3d876e61745a12cecfdd8b2f3ba2" },
                { "pa-IN", "0bc29f1a111749bdbf49611c9aaf4ca59e6b86f9943957cd57d8ecb8b810230b6adb26d4b3d3093501b1f65e9463f4c21607558c1dc0291bb9d02c016ff61e5c" },
                { "pl", "5d54a473de9ee516656616216ccd277fe47693f365a3995e9e65a89baa92d63bfe31bde30081a648d95909a1957a92e8135709730e1ec00212e6cc30ed2e64c1" },
                { "pt-BR", "018a17b7c28958e183539539295941e201322c4a9a1b76ad8e43c0c8696999d1bd22860332b5465e046374f3e071b9485eb4adb1290a3647a118ff6da0349d4a" },
                { "pt-PT", "5d7f2728053e8f8335e334342a919988b03e9e46044a2bbaffbf3070f1481317c29d7f93b3840a82307a316b2130a7b18688b6f5cd9afa32a3830f07b0e26f84" },
                { "rm", "0966d33d43af2ed74f2581d60733473f8bad3e3f3e5ae97fe07a8afb4af8c27c0485d0abcbf8c5f3bba1fc34e3722ac0dfd1e9588f1ca024176896657fc9b463" },
                { "ro", "c4a00f78f0eb1968748d4b827023adbb0915b1eb8ba41cbbc02e0cb42421a5246e637ef63b6ec343e6252ba673a491090aa1899c996d7d3083b8bff83dc97836" },
                { "ru", "a99f960e2a3f5c1c492d2e13eb9cf715d2b3bb24f2891cfae72dde52876037081528efcd3e907a70a0f2c7ccb8014742bb2c2771bb74eaf3b2472794f0536ad1" },
                { "sat", "49e6ef53def10ffc986724b575fc2732a3a2173d09ccde58cfa1f4b756f608f9b6e26b9884d1ca117a393d679fdaa9432ebd95127ba15b1de1345c2f9fb51c3a" },
                { "sc", "632ce75cda60dfc4b582a0f36a41478c60811c2af8b91c2399c3863dd1216c79f7dd11fb70439ebf16be1058fa82423fb95f7c01b90526a75802d34e93ee8d47" },
                { "sco", "c5c004fd456e6a34fc2a987e4be829a32810bddaceeb8ca74072c3b7646f6ba24c92cbd9adef19a1db03d64563b45eadccf072e6c5f98995104ad8ef93c4765a" },
                { "si", "c773e230628686d31e3249508d5b0dccba1aa4e36664dcdb864fcfda7e0c5e8dd75a77cee992f1f5f216ad679f2846aab81e453265fdc6829c0654df9b519785" },
                { "sk", "f6ed628d411d5fb9a0a3631f34d2748b15e851dc0f09789f58f3465951fc7f2ebf1eea108615ad68146b513b2d2ad1a0d0e05d25298bd92b06c11a3bd2152c60" },
                { "skr", "91536d214c5e4048a929def8bb2db5196ef14cc928916e8a5e2a9511ee1930eef526418680493d9d0e2b31c70285bed91da8c382725e0e9ee0b064a118297b91" },
                { "sl", "ec04030f980aaae9f9c2f0e1da8cd2c72c752af8df10e6330c1f1994f9eaf74b35fcadb1c925d6a562527b4f94ad73dca6bcdcd244109bf51a1114f97dcb65c4" },
                { "son", "fcd04d3c835b02fe27d08e843ab54eff3e2e5689fd741ad747e7c6b2a17d4735b3c9d37ef493dc2873267343bc70fe0f9f9c914a40b1d08aab998d0e1d0f9e5f" },
                { "sq", "1f0d38e1706d458dbc9e54805eb9d29f40910a681fd9e174fd1fe28e70b678b63e6dbdbe9ef96b5ee4417aadb261079ef1898befdd7a7d7ee64aba2ca3380b7e" },
                { "sr", "3d6423587ed5ea2755febb5f7d4f42d9aa7e301a0f321379cc4bf058acb24a98fd025008ce8279481ccd6ed1f28b38204e737f72eedf5e5909a1dddee1bd568c" },
                { "sv-SE", "1bd4ceb78b1cbc08f906c3c45f8785fc94e2255ace85562a27e83d6e87402d855df87389621ffa54743017adb3af98983a8995f59cc426d2497c05292256cbd4" },
                { "szl", "060955b28d800b100a7eed8c2eef347c270df4349fdd5b29be8626a2b657be0e6372a81197d3f42cc1a5b40c1c44a6fe2e72f2f018d6380528b4ed4c74745849" },
                { "ta", "81bbbbf70fce91c22f2a8b1fd59734a4bc8b8a72719d9e774848bbbb2f0abf296bfa9700b5c567f5595196d49c12872d3f5d081d5500cd5a2b65c868c23cbe78" },
                { "te", "9d6df0e317ec1d216cd6a5435fe75dc81f3b5ae46611178b0f66cd784f9381f30ea5fb868789b918daee1faad5ecf6e9df8a9c3d0eabe8fd7257d4b6f9ffc4db" },
                { "tg", "1b4d537fb2400decb20db3110ec4b807b7980dc3dec52783b72ec93317413f33ee115a3219d57e61fc857f2f959ef0d2c5f5a272bcba0d5a29ce5fe43748f2aa" },
                { "th", "80c49b3a4b30d88faf7990760cb6b1b822f806623083a039223fd71f6011164c46518802972bccc884c2ff7742ce96695b85a5441eebeedfeb36b1ae2d571843" },
                { "tl", "b1bbcfd25df68a8e5404eb532598aece3572cf390f80cd9736aaca89faef86afe48f667718bb5e3b5a41b90d536f6ce45d009b7a69b023ca75d93cefc573c3a9" },
                { "tr", "d12a732efa492be250d8975235ca24cf8355c00e68db143b48debe0ed64c8dee7284b877372d2d63d370020399913a67756218aead1e074ef4ff496bb810001a" },
                { "trs", "c0d2de65ce04cc6bb9ff737b74be703ce9fb4b0c1b12df9c6b2583599e5e0c40613485fb450de6e497e5a11fd9dfcf028aa4c4d1cf46ec84cb8b23f92858868d" },
                { "uk", "696170578f51d18ee3087786ad47b96747615e26a03015a82862db94da409b869ca7e23904025bc3fa742648350e9c547acad23ea9865b462217a0ef23ca479c" },
                { "ur", "ee2a42b445121b678224c4239529fbefbadb7e0c3f3a109515ce5b26b8fb34f03b1dd3de3cef46d680746c4e38b881710d411ae132571f9e1977e65247173bda" },
                { "uz", "d29d80cc4367254c020a06f83957baad0ffef0f3546901ad8defa301c044bfb0ab4ec788e484db8531791810ea5e942eeee75206d37cb4681a447e4bbdded2f7" },
                { "vi", "0d08425666d7ec9967fdcfdca8e13b8d8bb76389a4803977333943b694d3a4b5ab861d05478c77b4bd96afdce20a6c54ced6f2998e70e869057aa367358fc775" },
                { "xh", "744045eb56e28a2054afa6d62925de04dd56264cc305ae7da87c979570ed510a50a14e92f1533a9206529232ccd95bc76013b25cfd2af313aa88f5312364fd6b" },
                { "zh-CN", "2bb69b14b765436f320ac093d65c442cbddd2000be46ec4ee8570341d6133974358191c1e82af972784a9cbd2e7c46189e8cc32c614246a99fda784050f14199" },
                { "zh-TW", "5858c2354b2644337e27326ae4ed0328ccd17b96dfa15ab07c8d7c41f4bafac764a0694bdb1621e2b3c99124fc4ad1951a4a65daee5540b472105b76c7f57034" }
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
            const string knownVersion = "131.0.2";
            var signature = new Signature(publisherX509, certificateExpiration);
            return new AvailableSoftware("Mozilla Firefox (" + languageCode + ")",
                knownVersion,
                "^Mozilla Firefox ([0-9]+\\.[0-9](\\.[0-9])? )?\\(x86 " + Regex.Escape(languageCode) + "\\)$",
                "^Mozilla Firefox ([0-9]+\\.[0-9](\\.[0-9])? )?\\(x64 " + Regex.Escape(languageCode) + "\\)$",
                // 32-bit installer
                new InstallInfoExe(
                    "https://ftp.mozilla.org/pub/firefox/releases/" + knownVersion + "/win32/" + languageCode + "/Firefox%20Setup%20" + knownVersion + ".exe",
                    HashAlgorithm.SHA512,
                    checksum32Bit,
                    signature,
                    "-ms -ma"),
                // 64-bit installer
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
        /// <returns>Returns a string array containing the checksums for 32-bit and 64-bit (in that order), if successful.
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

            // look for line with the correct language code and version for 32-bit
            var reChecksum32Bit = new Regex("[0-9a-f]{128}  win32/" + languageCode.Replace("-", "\\-")
                + "/Firefox Setup " + Regex.Escape(newerVersion) + "\\.exe");
            Match matchChecksum32Bit = reChecksum32Bit.Match(sha512SumsContent);
            if (!matchChecksum32Bit.Success)
                return null;
            // look for line with the correct language code and version for 64-bit
            var reChecksum64Bit = new Regex("[0-9a-f]{128}  win64/" + languageCode.Replace("-", "\\-")
                + "/Firefox Setup " + Regex.Escape(newerVersion) + "\\.exe");
            Match matchChecksum64Bit = reChecksum64Bit.Match(sha512SumsContent);
            if (!matchChecksum64Bit.Success)
                return null;
            // checksum is the first 128 characters of the match
            return new string[] { matchChecksum32Bit.Value[..128], matchChecksum64Bit.Value[..128] };
        }


        /// <summary>
        /// Determines whether the method searchForNewer() is implemented.
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
            logger.Info("Searching for newer version of Firefox...");
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
        /// checksum for the 32-bit installer
        /// </summary>
        private readonly string checksum32Bit;


        /// <summary>
        /// checksum for the 64-bit installer
        /// </summary>
        private readonly string checksum64Bit;
    } // class
} // namespace
