﻿/*
    This file is part of the updater command line interface.
    Copyright (C) 2017, 2018, 2019  Dirk Stolle

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
using System.Linq;
using System.Net;
using System.Text.RegularExpressions;
using updater.data;
using updater.versions;

namespace updater.software
{
    /// <summary>
    /// Firefox Developer Edition (i.e. aurora channel)
    /// </summary>
    public class FirefoxAurora : NoPreUpdateProcessSoftware
    {
        /// <summary>
        /// NLog.Logger for FirefoxAurora class
        /// </summary>
        private static NLog.Logger logger = NLog.LogManager.GetLogger(typeof(FirefoxAurora).FullName);


        /// <summary>
        /// the currently known newest version
        /// </summary>
        private const string currentVersion = "69.0b3";

        /// <summary>
        /// constructor with language code
        /// </summary>
        /// <param name="langCode">the language code for the Firefox Developer Edition software,
        /// e.g. "de" for German,  "en-GB" for British English, "fr" for French, etc.</param>
        /// <param name="autoGetNewer">whether to automatically get
        /// newer information about the software when calling the info() method</param>
        public FirefoxAurora(string langCode, bool autoGetNewer)
            : base(autoGetNewer)
        {
            if (string.IsNullOrWhiteSpace(langCode))
            {
                logger.Error("The language code must not be null, empty or whitespace!");
                throw new ArgumentNullException("langCode", "The language code must not be null, empty or whitespace!");
            }
            languageCode = langCode.Trim();
            var validCodes = validLanguageCodes();
            if (!validCodes.Contains<string>(languageCode))
            {
                logger.Error("The string '" + langCode + "' does not represent a valid language code!");
                throw new ArgumentOutOfRangeException("langCode", "The string '" + langCode + "' does not represent a valid language code!");
            }
            //Do not set checksum explicitly, because aurora releases change too often.
            // Instead we try to get them on demand, when needed.
            checksum32Bit = knownChecksums32Bit()[langCode];
            checksum64Bit = knownChecksums64Bit()[langCode];
        }

        /// <summary>
        /// Gets a dictionary with the known checksums for the installers (key: language, value: checksum).
        /// </summary>
        /// <returns>Returns a dictionary where keys are the language codes and values are the associated checksums.</returns>
        private static Dictionary<string, string> knownChecksums32Bit()
        {
            // These are the checksums for Windows 32 bit installers from
            // https://ftp.mozilla.org/pub/devedition/releases/69.0b3/SHA512SUMS
            var result = new Dictionary<string, string>();

            result.Add("ach", "43fdfe687a0c00fcf30835edec529fa0d37c051396138a48a6542fd2a06262f84ed9727fd0ef2534a85a826b549a8a103f2c3179985e8da214d82b0a3660f2a6");
            result.Add("af", "53b2548b91a796b14a92d526ed86be621541ff299ec0a61eb08fae009fe111294ad4d4acf47b9cbd86057faf5069828fbe3806acbcae02f90032908a4e266ad2");
            result.Add("an", "18c54fc858435b69d548d5df897fdd32bb65135c0d7e139976f7e3a772286981260e19b8253a3f39bd490308893c4b2c9b43f5913413580af8acb81e3f6c8e77");
            result.Add("ar", "fbcdd876cecf9180ea430e98060924121f119ada65130e86c7568be0e97b2051b3fbc27973c392383f5eb1c0c88a5b445423db628e8fd9200b29fceab981e5a7");
            result.Add("ast", "794ba02f8568c2e0d5e4f37c0002d3eb67a5f8d3acd2846c4c35f87dcb1fbc14a0f9fd97a842984bca7abc0a8db321da1f9685d101dab4382f523ecc1f94fb0e");
            result.Add("az", "6090462813fd083dcb5d2b17b95400255f392416f6db072f4bd07df9b85c064f03d31d1ad5e75b4818eac8c8fce59acf60f21a8ab0cd7cd52b83e185a386def8");
            result.Add("be", "26b1c930030bf80077a19a06e174923edbb751f683e864dd9889567d6b819e3e3bfaad1776b6a9961ddb3b396215948bcff226dc0fcfa1b93a95a82626372800");
            result.Add("bg", "29a69b3e435581ebbdca47c3ba56d351bdf4f708e337183b62ba903216cb36ec2c518f23640d1d1f3eb3303172ceef115ab883318b135642f6e55248bd564b3d");
            result.Add("bn", "b4db30ff44374920dca89ab5548f55ce63ca0d2a6c54dd2211cb653b8dfe3c6fcc8a8ed73d7c066d73df973e0b37c82690776d6979bdba0ee49036ccd697914b");
            result.Add("br", "90a9b4add401da33b42a811153c48e471916555ba62721ddc9477aa3d6b98411ed4c1cd4e0c182aca8097bffefb8ec46b1bf342da2412d8880a5c96fad385e1f");
            result.Add("bs", "f41765db9dd00a6e269fe417de34633ac5060374b69b4e665af92d21a994663ab5e02bc2259cdb91fee67fa4b2356eb373033a53711e601ec709fae0185d5911");
            result.Add("ca", "834455972700bc4839af23ce4a34ce2589e17e84614cc948f6378bae50ebf8c6a55d91a860bf078cf5fcce7d52049b03c6a379b89cd1045f3f1499e52a3199a7");
            result.Add("cak", "2aa73262b89b07fd176c20da18d9f5886d1a883101c0a42f0c3cca6f70bf6b05bbc232fb2721c1d145529ffe2d2534133c5f4cfb3f77f36e66c0e02cffd6f897");
            result.Add("cs", "b75178616b7c1273ef5f4c20fe1a98f09b1466dd8c255aa99d5153ffba4b7ece3a97981c49e46e1bbef971ce51023d57f741882e4e3207a480774b1eef42eb1f");
            result.Add("cy", "807ba2c30660c908afeeadaace4b618dd3cf7646ee0203c787f637e290a9279c1b7975aa38fc07910064a4ff9c36adc57c67e4e99092ec56f2834fd41254dcbb");
            result.Add("da", "5fde2f6a5fa9ca3e3a5545693ccaa9e5c91d980bbdcc382a9eadba3d04cce6ec954a9a723f4dc98ef6fb7f7a603b4a36c34b8b721d4c4770b456fd9de82a06a1");
            result.Add("de", "2c526dff39b5c3af3dd8276b8a3afca4910eedf130b2a89a5b61e077ae66ce79efcfd89bff4aba8438157501e4246bd34ea69684624020b37b876611762c87c9");
            result.Add("dsb", "44a48387a8ac2f22a1ec83b0d4dcaf18d241bfccfe4680d1e369a65d863bb041ebbb39476c3d809fe74e02135e3aec7ae80bebfc0c316ec0060321595ed90921");
            result.Add("el", "a602bc72068dde535d2d3dba3ff38a8adb88ee323ac6772fd8d3a9d10058e43d359a854436fb694976f3ab0fb762c421760fbd2ffaf3bf0905a770b68e315aee");
            result.Add("en-CA", "dcdd5949dbb499558836b3d3917cad5e388cc1880cd07d259482c5bd9e79fb52a705fe9111fb06eb098737def9afbecd9b9e0ba3cec2dd6a1ab6e96dc08a8301");
            result.Add("en-GB", "8b14cccc8133e1a030c0a7aa714b11c1e236c6d4f5e1d77e317307a839fa5b9ad8631e04688b712e932eb84e1969e229f1f64b51c5ec08ef083af7924c244168");
            result.Add("en-US", "a105a82b44609e653b6b5475d911ce0b7445d9c83e110f8649aafef580acc364b1b47b69fa04935185d855f771ad7a02d3d1d5b2f400a35daea1a8f0c62edab0");
            result.Add("eo", "776e3fba04a8849b75cc91a3211bd8b04b5a6e0ee6fe0da3dc16afd580d819eb6504bd7d69c6a0f5d5e060272cf7c10a16008fc4ea3b206662a42687dee2ee0a");
            result.Add("es-AR", "5492a264e162bcb5a7285ebf301a42e64857395b75c9ec3c26369fba507ad4e99121056b2ca74f8544646da8557bfe60232b3eeecf0464530f4de53d3f3776be");
            result.Add("es-CL", "1dbe7130ae2cc2b16f7cbf4e3791c581fffbf13d1cbdd6281a5d8201c859a55e77744d5e23006a0d768238d664b7b3a3ff9e09001b12a3bb9d656a0c5fe71fd3");
            result.Add("es-ES", "ec5bd2163f3c1d3b55358fc74d4870c6221456513df6733138624e00a180c078ee0d6b96c907e4e781d1f6bb4b5bb39bc12b41562682419d1a0e9d59e1641b12");
            result.Add("es-MX", "93f42c47ec69677120bbe49a87d2dabbc226e113e0cdd1676ac496bdab55099338a567ee706721adecf7f6b7baf7b75fa0aab8e365828d8d703434b042e60a4d");
            result.Add("et", "20a65fe2b274342e8580d6ec46a28223c97f39de3e94bd503409db6a19211246c2235cdbd4d969d8ac87577e4eb8d964a9eb136d6ec6d81ad91d09bc94ab96eb");
            result.Add("eu", "8d2105c91102dc84f2fd59357ca01eacf385dccfed5cf82f8d3d332b995d7392a30c44a70d93108b707114abfa69403fb86df9c3b9ada67e830a823d184ac09e");
            result.Add("fa", "478b8cdd1e63ff2e56eb2074174da0e6be93e8a6c25d8ce9f9e29c0def321f68e5038fe235d65f11f1c79877b379af2677876f6b0f12ac524b758bca0e03d83d");
            result.Add("ff", "90f1d366c55017a3bb2d72af041e74669e835a3ad40daa036ceea4a3a3fa64824f2651eddbdb6348d98f4e1a2e3344ef20d107b58884586579efbf2371943103");
            result.Add("fi", "6e53e2cba4f78ae6831adf0e49218a889761c6132112fbac837af99f7eb1edd29576fe8a14f65a86a5b5a5029d69477880f848cce32f8432adf852a9e5fba932");
            result.Add("fr", "2bbdc2413316b266353908cb107ae575c444d9a67b3a9fd2d1b7de7947c130b844aef16ddd98d7f1340c4adf919b6d897923613920831450d0b2280c1b175d47");
            result.Add("fy-NL", "627d0cb4005a203208dff81794bcde282134fcdce6210948edf1aa5d7125d282da08cbdc67c339cef1f2367998f09a228dd3f830cb63d23776db6c8d433f9e2e");
            result.Add("ga-IE", "703cda56bcb2cbc415a5aecbec618c403a507236f051258baaf285e596a632659afc7343f7710d56cb6072e68f3286ca78ded4c01d8104c12acd59808e56d73d");
            result.Add("gd", "cbebb917d3b934a690cef3625bc6fb3c3767805eddfcd1cce3d7e35ac19e383f6bec57acd1925bf7ba8b71ccd1fde5c366dade58a4a2491cfc962a70c1feccaa");
            result.Add("gl", "9ea0bfa2cf6d5c729496632486a8fb7c445f5d03d6e5a6d16543253c46ac34b3a96149291c3622e8446d3b1eece2ae071513840f7d37ea40ecb073538b3cc5ac");
            result.Add("gn", "9e5ced0f8cb8f1ce5e64ea27d4a22cea4e0b2f97217b2b39fc7cdf0ba1d975b3f3c10a6bf6808bedf222ebb4910ba4a6158a75d7402bf71950399e39cb6c9ff1");
            result.Add("gu-IN", "28e1ab958e2bc75db769169f17b6318d57b7b8a942a7acf6990b226e9beee3270908ba92b07f0031ce112598f6de03df765a94fcd7d00c51733e0c843cfd9d2d");
            result.Add("he", "7ffa28f589b08081d0d0fe1b6ebd41b9ed4a1a2f431906573e41b6eb2d6cd8ca0b0db4ca87b4c0a25408c92ead1e2f85053b6a27e51915e5092a68132b354c70");
            result.Add("hi-IN", "a189e10afcbc09166c7ba70aeb23207090e894293ca4d45f930e2b227f579f01c743fb8869ab08b7cda82c569048585bc9d118bb0257a5013f1cec6e426be1b5");
            result.Add("hr", "71cba0099031ee67d025a1f5b9475244a2aadce365c7fe7a789c718c26b17f5f9d355ac7dec6fda82b01b52a6c5af9004d5078e072723b66951d3a4ff8beaf10");
            result.Add("hsb", "a1f45dd8c5409ca13ca09ea81360d488ee358ec346e3e3e7f11af06b36c5b7ffa6e55bf5094e4a0e7a693fcd6be394dbbc7db882380764b0f67c7fc974eb4f03");
            result.Add("hu", "bd28f3ef73c0a1eb09c52f38960db6f0f8585cdcf251478f8616fff2db3b1a7c39fde31e413ba38db9724aba70813e37661c40ad070b7989cf4e40b0222d0db5");
            result.Add("hy-AM", "d71b91cc3ecc6ee6fc447961d6d013e80b35e1c3b8171d609c449a3a2b3dc7eff54f28267759ba892f1addaf3996ac03e7f9c35ff409d21e616452bd05d211bc");
            result.Add("ia", "e82682bd829c2b05eb901dd1db8d7748d961d6cce1c797dad9dcd130b377347b007bfe6b4d607440cb14a4839aea47bc8eea5217d9e0bcd9a6d21f6f917bd721");
            result.Add("id", "9e62fead3bb4d0b085b07e5c144c2337799695448cb87bab9bf7e60687a1eef8403c81a4b094880b5f38ee4ae75919e14b4442dc277821af945a3d517ca1e38f");
            result.Add("is", "ec161ff07c0952beee99e67fc506fd1fa016c51835580c92a1a4bb2195e499c18f217dedade7ea0676c510023fa2b3145807221fe0c776d8fdab5561a83d531d");
            result.Add("it", "345cfba423648248efee2d6818042c5379d909da6bc45b7a9c64206df58b29d49abe131867f1da20c12e87485627f325ca7f5a5b6443e04e2bf8e34c209e807d");
            result.Add("ja", "7293e0c5d098a8d6eb75b0bb1320e2db57292daa7ce172e68cbf0243cf1827b6769bd2760cc0fe98ce1965abcff877d38011fe8a33b285080043e19bb1fc2e7f");
            result.Add("ka", "9a0a02d339d36ac7bb06ce153d67ef2a1a5976918d26ad7f5324749f4aa3f741388ae80e81f9cf95b95c0a6500dbcf792dc15ef2f457fe373d3352b42b83d5a9");
            result.Add("kab", "bf73cfedc6c94dcf012d32fe66245bcb951a026394cb83442e3fb44dc3724f2f77cb633d15beb539e60f163f55fdf148df394f9e63ee4a1ae90db8aa583ad972");
            result.Add("kk", "7651bc363c77ef424231fdd16a7345db7465b09d554de506f2afbb6f04c912cf5c88be1986e3f9c3ffad7d509d42157a93d9dc3d482cfc16036f02faa60768ae");
            result.Add("km", "43c8cf0ff6c67514a8767a74d5cf0e5fd51ef523d66162f62e1fadb6d89e7194a5f6c7820cd4e34c739e097cc6da6e22ad59b5748ea0deea10dce33dd3118129");
            result.Add("kn", "efa25ee8a958a8c60c2b2990066fe43ee6ac89cf7c4a5bba0fd13551991b76cf56a987eba8ae9b2086fa154fe2734bc30b10d48fa7d6405994c7cc4f6d353cd5");
            result.Add("ko", "0688714515cbaed693d8c644760205dff7e63552bead62b32cf0483df0c0ca0ab7255df770090179c0c2400287479436c34aa1704dba8ffaa38326b21423bd7d");
            result.Add("lij", "dfd441211398407d1b02775583dd6b9eac3e3a5fd86d73eb2f2ce70d5a4e8a897123e884efb458c87eca31d4cb36ecb409c4640b4c3fd10af38120cf2dbeba5a");
            result.Add("lt", "82e64dd9c59a5e071de29b1a6b29fb53e191b520a7bd3b45de2ccaa176eb99ee364381bb3702a20c913fd349d10b815fdc4e2e932e787e66d6322b2efffc35fa");
            result.Add("lv", "b2e42f1342f1da782ba0bf90670fa3c91aa7652015a698b59619e944a0eece5cb3c28b132e8f4b8ac32393d31b61435c0f89cb8099b729ac11154d2258c8900f");
            result.Add("mk", "3a3b7e35fd3cc915fae491b097e79ed59a48e44ddbb1ebc6edf449b0c4d2f6a1b639f740dc7f7efa647de8070ea053d71aa0963a702d9d1599d22a3831989cbd");
            result.Add("mr", "d7c94649684cb7de4f708a284760ed45da599bfa3a140932d12d47739968b88a7edfba510a752c5e4dac3d036fa372eea094e49d91afdb0b591f616343b492e4");
            result.Add("ms", "251ba7854986dbef062d2062d3a7c66cde9b2e9a359562b1e17bff00ae1908771b73b73779f44b5c91ba1e6154468d1c312f97222f8304addeb670ea41ca2870");
            result.Add("my", "e6194f01cb1f07bb68e128a6acc92662fdc91b238b9b581e62ec5f1326cc959bf2f4a6580e946b6cd9bbafbf7878c8fbff97a6ca047d3832b6f76611f51ffd16");
            result.Add("nb-NO", "9d0ea4a8e8b4d787cefe91becc8d628e9ffb9e86eeb8f4c3aab9637e059d9ef01eceed99281dce3c7c4783684616c83d6d3c4c5904d79ec3fd1c4d72dacf816e");
            result.Add("ne-NP", "a187fad2d98cf31cef4987546b3d46df2b67e21d95447684a7e717ba8fb76ebb77b04da4e2c7e16b11b51ec8ad977e671420a18d9bbb9116de2fbf08a998ddce");
            result.Add("nl", "1225b6abad6bb846b8794cc9ece06f387a4385a2ba883647382eb436b257549153c5e194124a781917139a28ed9050575e0a37d0ad37196e1c246b5b4645b1e9");
            result.Add("nn-NO", "f367f75255d8b318554faa6b6499ff0ce0a04b9d379c3e7aeb727bef0f528a8620f9da15c9409a38b57026ffdd847096c6e70be7243f1734f5180aaf231c435b");
            result.Add("oc", "6bc623abf80cd0a7de03bb69b64d76505c8e43f3488e85f227e5f807d3ee62c7237090532b310ac7b908d14690f460f66c119b904b796fa20914a3b36068b631");
            result.Add("pa-IN", "5deb8f79f43a85972bb80b989519e143edb91096ac33a38104cb9498b4c91f271a914ae1ba0baeacac2165760c425b0596e326dccac9c3572f72c329131842ea");
            result.Add("pl", "8c48e1d40588fc5048e485cf4000aebae2d6b6b6a2684390c74da3a4aab0c4e3408d517d7b218ad864b07e02dd9e33d6a7a33bbdeb9bb0c8f3ffd7f915a203aa");
            result.Add("pt-BR", "d6da7e208d8e88966e60867b71c26d0dc7032898a9836f545207a2899feeba3e99fd6e5848b395f43381655d54006fdd5b4513c162034c20b7b1ade54f0af9e3");
            result.Add("pt-PT", "294b91ccd90aa747af3138e4135a878189260b353f84b9e597938f026cce49b7c77d65d2d2dad1261c56f1e41551074047c67ba94f5aff202ae37c4ea9752b23");
            result.Add("rm", "cf52f4c628fc73b67f76d265866ec6cfed80ee7b84d7aee4fdc165b8d5ac22f0779f48c3b3fb57016dfdf932acb2a1b729c04ea7da95a2bd997d29b43121f7d4");
            result.Add("ro", "1586abdf3edaea1dd761c0bf69eb5d2949b58ce23197dce77c7a74df9c6672ea457ec94b7a4c2fe5b2622daf2ff6f3d05f2a874862f34b69a73b29471bd11fb4");
            result.Add("ru", "cdd44e4dcba6319a8c67a0626f70642ad8b473fe157840f5f2e3f3172480cb47ac9e413108b45aacd222cad5638be7ff297f6260a8b5ebb48dbae42371636070");
            result.Add("si", "cc4236d85a042281a9749a4b971e59f77aa6991c11a7eddb6f7e57fc258384712aec6e27d7cb27fa2703f895e2930cfb85d2fd759d66740cbf93b055be451914");
            result.Add("sk", "a93058e89f671d7836d4b13e573f82dc2372c7e7bf62836f91535f0e60d610d6c141aa9c5a142a4082cb4ea648c5ce928f808cecd47737062ae40224bee9ba5f");
            result.Add("sl", "279855a8429980d4f4911ece710c3959b7a362537d53caa25cd9ddc4f2e9baa6d435c7afb819c8e8ee839c22860d9814837285d45e0fcec6aa5a96417b1c6759");
            result.Add("son", "92b7010424423b65f0ab1cc2960ea2207f770b82f27a3dc3d2b7aa95cffee8d1c18e520e9d9ecdb0251e0d8d9c7fa4b13036d2f2420dfb019f1a78d299f31b73");
            result.Add("sq", "300657a4f0d0962f8548b6def51a51b08fbabb73fed700df7bb11401cb4679e6912a3a3ea54b1f72d83ef4ae20dc7da715a75bafee6e6540cd7bcdf7343502d6");
            result.Add("sr", "d2c8a7302a9135483af0358b652f1b08ddbf0281006cb70e251a1fe0bc2b7548266ef7637433bc2ffcdf8dfd156ba83afafc2b4270d64ab64aa24bb8740b1a4c");
            result.Add("sv-SE", "2ccad43b61ade39169842ac6a507e608f4dab51c1c6ab714f58c385b9671cf0afd5adcff5ef5e2dcc704cd46e348f65cfaace75d4168c8b0e87dd13b4d429eae");
            result.Add("ta", "9f007588429af9d7de765151407b9cd6139fefd7062760807cad89c10dcb8786add8508971cb68c16db7f5a01615ecfd9defcc0df9dff857bb9c754346fc2b08");
            result.Add("te", "eeb44179b0227ae4754216076ba31fff0fb2a8f84ae8bf9a3bd2c98313c1f5aa5bc80dada5a2047c799ab148f23e36351eb07abc4b422b79df98348d78d1a9c6");
            result.Add("th", "c948d05f59488bf6f3ecf1ba7b241828ba09cf3842e9c5e13a233bff2e510b0da8cf0519b071e2d46d17ebadb92f5adb944e2045acab228c98e5e226b2be392e");
            result.Add("tr", "0dd2ee9ef062ba866d8919184af973e9e1d58078d372a9e2ddf67c8acaa0221a04cd2226cbf01c2a5af866d7e3636c2ab04aa7598a2f68d9e72d941bb481c255");
            result.Add("uk", "02c5ecbade24bbf06f72b68889be3eb5f4092e8a5e6e08612167a8ef2489e2b73df14ed9a9865e88b1b8e08b0901ac01f5dbd3c9766c2b39dca8f6fa8fdafb78");
            result.Add("ur", "5faab67ebe0953adab62cccdd210855c219dbd374bccf0a9a8918b525c3697a1053c5a39621ce9e687b89e74099b4efe7f0bc907744e85d004153de09ee12175");
            result.Add("uz", "13ef1dfe527e64b1279ded21b9fcbf5e1b70633cf49d2809b08830ff75340ef7d8a0f5b1ad224725c0c39513a4b019b6fb66463f8f9e18f7ba29c2a75e787a1a");
            result.Add("vi", "f955f210e231e338f8dea13bec7000f810fd2355c9dc334cc0ee4de3db0d573bb97f8f67f801605bef01d54f7f85955850b5f185d3d4de7c92d3a5a4540f1f75");
            result.Add("xh", "2e30249196b32c10afa2f6ffa19b707921ed2a00d96bccf78a2c92ce4cd399c7427489228501df4a017c722fda71b9ea89f9565d47ebc9c93e01f13294530e8b");
            result.Add("zh-CN", "2f38c5c4abeb492b0759f0100ff54bc3ae3b179f54c6039539e314b2a553bf9a40cdd631f42af101a8417a61bd9d79e0b3a2a07faa715256ce29262dc9d4296e");
            result.Add("zh-TW", "cfdc950619a9badf3c88c7864b1f830ea3aefe52757d3b03c3441e161fe5ce9676b3e412e99c0d88abdd6c44883506d3f0db1d6afda377f51b5ce7abbadaecfd");

            return result;
        }

        /// <summary>
        /// Gets a dictionary with the known checksums for the installers (key: language, value: checksum).
        /// </summary>
        /// <returns>Returns a dictionary where keys are the language codes and values are the associated checksums.</returns>
        private static Dictionary<string, string> knownChecksums64Bit()
        {
            // These are the checksums for Windows 64 bit installers from
            // https://ftp.mozilla.org/pub/devedition/releases/69.0b3/SHA512SUMS
            var result = new Dictionary<string, string>();
            result.Add("ach", "ff5177674c1982c1e4eaaded9379042418d6a93118370ac59b7004afe3b7afdd4a574e2dce260f7267358eee52619a90d64ee01ec38953ede4d88bab7eab0e15");
            result.Add("af", "542a70a2d04c3f67a9c08c6389a7c9dfde59b61573935f7b6449814facdc539545066ffeb0f203944de763be14dcafaef9f9e4908cd81a53105dc48160431c37");
            result.Add("an", "ffe63fb5fdcf0a994ddde50b02e3bc06db48f59690bee2a0d1833b35bb974b14b21d45d2f2c89d9bea9b8833b0d6d53b388dd463b57441e83c9dcda38847120c");
            result.Add("ar", "1962e4a11542757f606af5115e8b557bdb1a844d090683c3abc76b84878ce72b77416f51542a74c36d49d7b5ed9bcc022ae3c1345ad730039cef7c7d927488d0");
            result.Add("ast", "10663b20a93d5fb5ad1c3ba4a5c8f8a88afe05cd7bc24c12fd0267d366435965aea1081ffb94ea1285314cd12a528f99fab3f28c346297b85d378a59e796c0a3");
            result.Add("az", "8491d4b882c3b7881763f4ec7e6370df3ac878e302f55c056595946e7b019c631f63eb8f0ad048fcf152c41b6b4f4162b6866f062effaf5fa6ede86cc1c0ae15");
            result.Add("be", "412f279d625d1418aa0db26b88d5e0bcd62c92b45b654d94817c33faa4e2951a712aaaa68d4caeb1fd75142b8660aa7516768e5239d398546c29c573e459acb1");
            result.Add("bg", "fe12095f2d7efc469a1208d9dc614e71650e4aa2c61435972a7afcbc56c0af96a639f75899fe8ddd405ca3a69222a6f6a0ea50147eff78d0366932543eb0a382");
            result.Add("bn", "386f92e630b124fa2cea1436c56f4855687185a322446afdd99718121a9ab9d23ce77c0e60641b81a57ac37783b4d4764de829d5b5276032c8b60ae2718ebce9");
            result.Add("br", "e2f27fb07d687b30c2463af41cf28708713849d6e287845e865c9acf8ed3cdd308649f424539ec0742cae69e223e2d120f276a131588cc08b2ca451dbfbe2eb8");
            result.Add("bs", "fa9f71a813b2a307b86e4482884a1bec5446b60ee41b8b38ab17d2d63a01bf204518be5691ef32ea37e1e3fdfdf4ab1eac2d31759248bf29d546da6baa490b99");
            result.Add("ca", "2f3d04c1dd1d5c4ebb4ff8ee05ec326f61127fd3d9a440c247c66474f7ed49f6e85b59456d59babf1f7a162e11023491aa59e66a02e51fd2ffa83461443e09f5");
            result.Add("cak", "b9c38a595d6abeb8f7b2baeee09837f04825e13df5adf1f88da847db9f32e3bfd516882c31d6269c0dddcbaf3f852c3deb04a3c6a4db58ad1ee8753a38c40207");
            result.Add("cs", "5095263e1e2af2996ed64c1b1f197ffcb56811c3e6836bf9f0dc06bfee6f57c2aec51d00b17db8032e10d91cb6dca885b9b48e8e85a81e39796c7fcac74469ad");
            result.Add("cy", "68337cd8d10d33be6dc2daf7fe9a62c4ea4d3d20e36f624009526a92a92a76c8e9155c2cdcc37ffbe11142c2f894da8283fe375d04e703e1b8a84e5a48e22b59");
            result.Add("da", "ad9326817d4a3f264a6e4189ab81f3bf46913442035849f062bc6624057eb89a715050174d240fd98b4de0b8340e5a125e27297a84ae2c4656e581add8350efb");
            result.Add("de", "ddf7eb7e98b4fd3aab05e450dcc59c79c36c472e76e770902ffa8d9e2897e3f2138027233e1d64a0144ec1d66606b182f107ad0baf5562b20a230ffa076c0366");
            result.Add("dsb", "0264b293a26f10586cf74b3dde0feb04066a6612fc2a15ae7671cd720300a5cb4d9e01cc840da8dcc078fb1fbcb22399d276ab73f7f4ab8ae648562078ed609e");
            result.Add("el", "1d2014f263ed58b015fd270983925623437eb9484de8fc061eb25f38d893d8b91d50292470348f628dc9dfc8f2155546bd3bcb53802458be46205933262f9bfd");
            result.Add("en-CA", "e7fba2126600b8a5babb60040622265e6976db93877c583f685af2839e336e4a3c1a6b7fd20eb7fbc83ddf917ca712f1cec459488a5c7750bb2bf96352e0f09b");
            result.Add("en-GB", "02998199c77fd4d39c292117832e4c6161c73aef8e68fabc061c25cc4ccaeedb930bd3ae371d5660ecf57617df7cb6377f5fcacc51a717905fbb4d8f8ce38539");
            result.Add("en-US", "074e160a64d3ff6361455ea7a0313e3c97d2e054adff7535138cf6d9a68b1daddecdf0f64925cd8c73c290012877e71a98914c256e441f9d1d440f3e2c2b0807");
            result.Add("eo", "9de6dd8c48e36673704cadece3c12965eee2a4c448df6e7a925f5ac58319586f032503855b2891b4c98760f1d4365bddf7a5d596f1792d9b8524a116c02e8cce");
            result.Add("es-AR", "2d37e1a82149bfc7dff02c122aff2340f1df53ce79b15d58147a0f014e17dff64adef131b987fd42621c3fae0ef0e3ad196013c1e6397eb844dda05092b5aebe");
            result.Add("es-CL", "61adf915cac5fc2038668faed6112090b83df435a0acd1398242543f5cedf1907781528a69ee258fc7bf78e36a9e5df684df9c84cb5fe2b98d2d950351a4ae73");
            result.Add("es-ES", "4e6f6f6b0100051d3af9cca2891d473807fbd5e04d0cfadb63e26c54893f94c9eec6f9b85ced0f3d56b1e4597741080f19b5fd0533d00fa0a5723ffb9a0d5495");
            result.Add("es-MX", "f378172f4f0a6e2e50d04be34188b28e8f2862110ad00b1e1ac2d81f2dfdc419c8d36c64b15c30eb2623d70dbbd1558703b5febae2239a378df476f755e4bd2f");
            result.Add("et", "80103006bc58c2189da3bf922d7de908078e8f3804e1b4ff43fd712486168d45ce20c5240d275d3c6e8396109083cb7495b4e386ec5cfb16029c50d4d586e0c5");
            result.Add("eu", "8f5ad6b4554aedddca822f243ac0af59a7f8f753c76035a70d024547e99a14a3c91a9d20b4bd331677e12198ecc66359d7d5be79340d38858ff8c75be2a2e4b1");
            result.Add("fa", "8a89fd8e0ed2d40c8d9f2f5991344e8605997cf6ada9fed4e5c1433954b2509a0f40c4e2f603ed718c5bb6801cdfd721b76878d4c556d60869bd03317c0aedbc");
            result.Add("ff", "23cc0489a6d3cc29c3c57adfec6d4c70afae3001e788cddf11132b4fb48ced62532557f57018844f37ce8373ed9532fff7add10165722ecc23bfe8a7a9eb8f5f");
            result.Add("fi", "f0b20f874f4037ad33b85435a55de5df5e6b57659cfb1ffd9bdbd62219d8ce302bedef76ac9f6d825945921cfcc0100564baf0918655fb3c8710ea6ebca2132e");
            result.Add("fr", "7e64f6cc7b2505989fb7381b8a11e0c9e0b73763bcea0fac74e9461303294c10f483020ae7c9afd8afa814460c122dbbac4864c2a48f897a799bd403f7dedf77");
            result.Add("fy-NL", "8ac3594e7aec3203bf41f779e4c38828a6290f35600165be6e57954b788865b56e66fc011c1eb3a04fcebd1188c730d8a02ef01df322fdbd5ce8333abaec366f");
            result.Add("ga-IE", "c260d3a31b36dd25378a242b1ba0dca10818e756e332db68ec51741430567e5b704669ea2f17b35077077c408b68b35c18e2a57f219db34289a97f5fff09b27f");
            result.Add("gd", "f03320325569a128cd55fbbc8ff18ca882f086f67b87a08aaa675f0f4da2c51aea95136693e2a17822aee052e20d82aa06a4757d249e861b71b7f7a73cc9455e");
            result.Add("gl", "e6645b363f31b7f3894828488907e5a45f7e1afd154408315c411cc9a643b4037dcef1af4e7aab82a2f578f030ca64d8249f6f2eee7cf626dfc6db74169636b0");
            result.Add("gn", "e53efde829f566345447d528317fc9dc8db6e2c2a707fbda436ea9ee8cf03768622d8fd4fea7e1379d58048382e16f41c5898cba2fe03823de4da18ad78ed70f");
            result.Add("gu-IN", "fdc10235ad3eb11905e6565d7d84d6008d17eec170c94f37e301dc5671689689decee4d2e2f5c3e9f931edfd93a26df36ab694c0e3f26e5ae407bb664406bfe3");
            result.Add("he", "4fdafbb4687a2546f46c154f37e5856fef782b12fe7ad04d6bb9ec47bfe6b08314838fd25d2b8f062d936dd03f56a34236cae2607149768bd11e391ea6408cac");
            result.Add("hi-IN", "2cf6906955057f4ad608153170ad1f0f6f9c9c58c79a1cbd0fb37d890f3f214c0375e4baffd5f01ff02c6f28deb5af22a606f975370eae4e8d67df3fcca8d06a");
            result.Add("hr", "888a60e428dda1230e0761df9a0f09c6f6f4905ca1b48ad68153ce25127d591313bf21ffab2e991f0d2444e54075f8386da44151beafe1fd2e14268e293f823f");
            result.Add("hsb", "594d828ffb77518631e6be7232e7ce3eb9bf1a2cfbd67a2163877b375feac36438aee18da487363f2bf3b3dcc873d539a00bee178d842c494025ed38b101aad4");
            result.Add("hu", "cc132ac054f80df3c61c4d5929b924b254355bc2729a842bc582c8fb9f0479c7aad4db8857d2656875be9d5e8b45974552a42068192c5b5872feaaf0b16b0429");
            result.Add("hy-AM", "d09d7addd19af561360ea2d86f00dc59183c7c28181914effa394647a58407c21f3a61eec3a16b0950fa720eef2f984d2d7a7088b58ec843275ed5c8d2ab2f76");
            result.Add("ia", "f92765ab33ec0658c1ae8ba4d0d6718551333e73469bb35e63804b65237efde641dd0c7b59eab197cc502ffabb70e04afdbcde510b716f30471cffa829b21b37");
            result.Add("id", "2e12943eb06d641cb880e98ecf6e41baff4b50a6b356e4a4b552ee274c0f8aa05ac03bdccec6f4deb79a950264acbbeaac89f1fabbbdf15acacda33735e835ec");
            result.Add("is", "ab6773ad14759b7758d3b64571100f5369ec5c599c71c8f61adf31182dfa855e74a70ca0c20c49e996cec214423ca6ed85f3e5db1b0eb6d3936095dc17e030d5");
            result.Add("it", "0dfdecb1b3a49198a96d146ad880ad92a6c2237d17e625530c160697720bb419f2a4e844b9dde52c5d5748e3bfadd50a10ce0fd51ede96f3849bd8b8f41a0775");
            result.Add("ja", "3c2af4affacace5c4a12472db5a63ec66de61842012c3106f319ae3abc6a6fe97bfa6e7065c6c5da2498fe932809c5847e8cab7c8761a12272cdaf674d113e47");
            result.Add("ka", "7dced27ab628a8cb51c886b6f41fc333771b36d67427390b457017a96d2652b7e495ed5d2a730de7a0ca21db358f6f22b26a13ec47810aea574423e3f1f86d20");
            result.Add("kab", "5ad9d2e3f6a5c6337062ad15916bfc298939bfbe7ed298a35398d36768b1065d5ad27d05184fc0240c3fb4e6372b3cbec6dcde464b5f0ce3f494a96b25379efd");
            result.Add("kk", "3f92517e09a78a7d16352ae81d329824d3fd1aed6abf687f95917d5e14d403f1df61096ba925c6b3e6d292d7c4498ef9c21ce249d18f9d6e6c089b64a3cdaaa8");
            result.Add("km", "3ddf9652bae98ba5260154889117175e9ba05a1bb869b6a3701909caaff69775d2bcbfb8b95ff7cdb428c214aa6d4991b66351105ec248d40f30eee0de7f1092");
            result.Add("kn", "da327c21bc22c03490ed2e8541eaaf3b53317bdd74e2b77366e57b4f0be3fbc0adacddbbe8c9df17c1e3042c41bdb88b7dd2892264ad957b70f9aac109d823ad");
            result.Add("ko", "39b3feb2ea549a8f0416c353efb89db84e8b50d5abcc46ed4efbe3fff66504eb9891ca0e751fe49f62f2cd573708ef2a4f81eb6776dfac85b748f889db8f7636");
            result.Add("lij", "9be102e53b7e4856166f267852ad159b597fe92a7899e3f04702fc24de0cdb013a5aa3714432f767b25e123a6b159325ba4b63813c3d5a6a4529fb2082b597d1");
            result.Add("lt", "1e5dc1c24dfa278c25523b8ccd1ad77c80392b06b92839cf08e40c22d7b8ef92abbbb7f84406a74d1ec7758917a0622c6d0c5e2ec4d182757ae26d2a1b8f6a09");
            result.Add("lv", "59cca71549d59503cbf7bbe7bb41b9810d4b0b428be7745a7d513d13af0441d7e630f18232beea9d963f8b2d5c54e7905f8e8c5719d94523ad67a42aa40a3157");
            result.Add("mk", "0c70884918de9a3cbd37882971ad06d46ff51ced8373d43145d88aa43eaa20c3e6469832d47823927b60e8a6f54d4f86b18495e0b410406eaeea0997a3cf4840");
            result.Add("mr", "a22ebcf3dcf566ada1f7d0755c1dd4b825ede56cd2c945f509e679eee4d170a3ad40861a225f3dbe07a1312e445a015c761c9f9963783989e2e9607f29964d41");
            result.Add("ms", "ccdbb9f167c2551e5b05d51a9ada0c1dac287ba22348e6e5b0678464a15bd8175c889dbfbfe1d072baae93ec8b3dbea9da98b5ea987bc29bba6091cebd3d2b68");
            result.Add("my", "61494ea01f2b41e0bd37a2e278a31df8cb565674213a5c1660daadf2508db154fc738a4a63965c9b9e00dd6ccee20c33202bca293a3b051a8865005b5d084a12");
            result.Add("nb-NO", "5c64e8d89230db9042c91b9f3a14c255186ffc61d60bd46dd4e1a2a7d2ff7414eec617ddba67bc884508cc04d79895fb306e86a9d4f321b9bc96eca1fdf85078");
            result.Add("ne-NP", "ff08d1c5fa2d8e1e0a6751e821c9e1fde04660097bf994493c43e229af1df349648c0696115a80634ac6dd12be548c6e73df407cb68a7b48598d92bdd8f493f1");
            result.Add("nl", "1388e1131df12aeec4107c2769c73110bffee794446f0073d244148c40d2ae092e134fdc6822005c963e5457f95639af81b427007bbc9c1010bcb5d5787b8b51");
            result.Add("nn-NO", "863b7b3f704e49b1a2414a444b3149cea5a81bdc602ceae4b6d340c292fc9237482bb372e9223d8f806739e7457e472ed6370b073808cf4d54235eca432bb18c");
            result.Add("oc", "47b82857626c5c32e919473807d3539d5e6ff8ed1e187efa61861eef8c58d27052923aeb93190adb0cb32b4e17db8d219f16f95597379c5fe08a537908ae5f8a");
            result.Add("pa-IN", "4a8c24cd15af9480953bc21d49edd2d0b3013427f005e4fb1dcc670826a94945ddbe8d4e89a73e04ce2cd12de91a2a5b3b2d1837331442f1e89868c7f8907116");
            result.Add("pl", "82376db152bc9d93bc364939fa6c7af4977d496fe6592fb4565425ff2972fc75f27ea5ba4949253b17eda1d18b7e4723d9ed6bd3335ea41beff17451067b1d39");
            result.Add("pt-BR", "67e463125267deac72fcd25d31e09acafd0b70b5fa8f86dc4cff34b2c1ef761aaae96187d17011ad4207d14b2e9a2d4c202168acf6c3ce296739c7ccdddbad6a");
            result.Add("pt-PT", "6c258ca2bcc783d25503f4177f8af29572d01f63843f95970176b95e79f753d6a19beace14b85498d725fdfca48f369e6e49eb54901b0fc54f6ec6071a8d5b26");
            result.Add("rm", "2bd7e84532adcd0852555ef7e913bf58cc668e9b4de42ec6cbfedd2c6bbdcf3c3260c03d1b854ec448c9d220278c3d209f72ea97012d7d02aca35d44383d8834");
            result.Add("ro", "acb1e365256fe3a2d4d17d39c46f34186db3d2291505c80f7d24fbef999978559146d79d59235d07998a37456a1759abf389af8245f7e5e0e233dacebd50dac3");
            result.Add("ru", "2308ecbc39854b2c2bc7ff1644e11f99409ce7c77e839448c7d9eff680993e743daf93e852bd28baa0a0e1fd92075d990f941844abd952efc13b171fd14183e8");
            result.Add("si", "2d03a061e38a785c05518c404eaffa7f95375c8a2dc646fdc40df4a52a73c0976c2b4d3f67c02b7fe69c290f2ddb7d4a35ed3dd2091911f76fcb6fa8177062d4");
            result.Add("sk", "040d4fa18e7520c1ae94f86998539eae60ca5e519113496be97595f1cf784b21593eab43a8f53280743068e943c60beb7f2a0f535605ec1235034ea1de1625e8");
            result.Add("sl", "f7b5ae28dc72efa408bb9b231cc55c7d1e637e4b10906fb4d212d5a7311009e7667a1c86f2815454622ed9f49295a1781c1c40e4677e039e0d88c3124104b111");
            result.Add("son", "0b04c22777d3230b76325482c9ab7e6a1c1d88ca6c22c0b37f7bad56888292d7467b7705ea55723a3aa32d0fa32ad7a26849bf063b51209553b86eab32183d83");
            result.Add("sq", "bdaadc7a311bc3c488ec0c30ec33ce1699ff59e498007a1b863430c149e1f0f426e26ba6fd4084fc4a644ba154903aac07e8dcc69fb9b178f114921757539095");
            result.Add("sr", "8cdaff6e913065ae4c7663eeb6ead5c832125d07bf8b1d65936f76258ec5ee090ddcead9cba0a2a60d166a37a5d6871a2ddc0c93b556cbd80268c0b7d0743966");
            result.Add("sv-SE", "99ae6cd90b0b46538712aab6c2fa46bf7603ab2a71abbabd4d96c7bf93b60f9a5a93f7834c4c705bcb966f8000b155f53dbf227f8399ca15ef69ce2f7f49d13d");
            result.Add("ta", "e8206c4accef45e873c75b502842062375bbf5de64c73d12c9ad1bd068ad8ac6df5ee39aee3959ce49b2ca6763c4dad93cff6f2fae35332b6ba694bedf58cfd0");
            result.Add("te", "1ab617effdad26ddcc2467ab66ec20254863b774e2a696f72aa226fea38562aeb6e744240a8f22b81e974dbf59b557a4e748d519690187a2fd11b63c8d109c49");
            result.Add("th", "3f519e28cff0839c23d41da97387587a6382f3790481ace5d97bd1bc893ca3b76f908796a1c8436df37faa48f87e933781869e12b5346fad2459d9f5c4eafec4");
            result.Add("tr", "72a8536950d5dc893fe0e6e6345d8a2f44b448f4c943661eacb0de603b66e86a32400dd71b50be7630b386283b1f619a31ce08ed94a810b2adcad028b8bd327a");
            result.Add("uk", "d68a1f86b145edc9d394d4b6cfae29470c6bf454809a1d265960217bdd12eecd42e8bebc32eea20fb11a35da6844ff803c383b70095f6fac565404c0e646e33d");
            result.Add("ur", "cb6fd8ec0a3dd73705043068dabd0164ca07a2f95b32640ca26ffb11fb77d4b3bb05b1bcf8e77850d2b345c6a2fd19b75db88d45170397f5f22934130ceb575c");
            result.Add("uz", "25b101de6ee1644fd00a80b20e44443540c4956dc73c445c38307b7dccbcbe5fc16af8bb6e630dfee5367c84c69ecfb8563c151bbc8f73ff93371e1b674b5c30");
            result.Add("vi", "70b84ed1d31c1557f11ddcc414fa2a0cfc70a094ae311fc363c8f25f027ca1f12ba6a6fd92eb985cbe05e3d414604f09830a34d246a530981340261b5816685e");
            result.Add("xh", "c63bb391861acfa377d5b0d978c1d73fcc21816ace27550f427549d5342e684c993b788077a027597536e2a4814c67b23304e8e60c69ec885a432a22b980b955");
            result.Add("zh-CN", "27dae05a83e6bbc23363e16ecbdb46bfd7d20b00012c990495d9dfa07e7650f166aed21d2a0f7cd229338c96f5667be424f9f37560e515f77c8bb6e032740620");
            result.Add("zh-TW", "65c1783716e23037038fccb1670e0d8ecc674fc61129a23073bb69d24db478d6bdc6727dc8dcb88e200e334357e5f276709caa1b14cbb6efef6e5d3eefcee91a");

            return result;
        }


        /// <summary>
        /// Gets an enumerable collection of valid language codes.
        /// </summary>
        /// <returns>Returns an enumerable collection of valid language codes.</returns>
        public static IEnumerable<string> validLanguageCodes()
        {
            return knownChecksums32Bit().Keys;
        }


        /// <summary>
        /// Gets the currently known information about the software.
        /// </summary>
        /// <returns>Returns an AvailableSoftware instance with the known
        /// details about the software.</returns>
        public override AvailableSoftware knownInfo()
        {
            return new AvailableSoftware("Firefox Developer Edition (" + languageCode + ")",
                currentVersion,
                "^Firefox Developer Edition [0-9]{2}\\.[0-9]([a-z][0-9])? \\(x86 " + Regex.Escape(languageCode) + "\\)$",
                "^Firefox Developer Edition [0-9]{2}\\.[0-9]([a-z][0-9])? \\(x64 " + Regex.Escape(languageCode) + "\\)$",
                // 32 bit installer
                new InstallInfoExe(
                    // URL is formed like "https://ftp.mozilla.org/pub/devedition/releases/60.0b9/win32/en-GB/Firefox%20Setup%2060.0b9.exe".
                    "https://ftp.mozilla.org/pub/devedition/releases/" + currentVersion + "/win32/" + languageCode + "/Firefox%20Setup%20" + currentVersion + ".exe",
                    HashAlgorithm.SHA512,
                    checksum32Bit,
                    null,
                    "-ms -ma"),
                // 64 bit installer
                new InstallInfoExe(
                    // URL is formed like "https://ftp.mozilla.org/pub/devedition/releases/60.0b9/win64/en-GB/Firefox%20Setup%2060.0b9.exe".
                    "https://ftp.mozilla.org/pub/devedition/releases/" + currentVersion + "/win64/" + languageCode + "/Firefox%20Setup%20" + currentVersion + ".exe",
                    HashAlgorithm.SHA512,
                    checksum64Bit,
                    null,
                    "-ms -ma")
                    );
        }


        /// <summary>
        /// Gets a list of IDs to identify the software.
        /// </summary>
        /// <returns>Returns a non-empty array of IDs, where at least one entry is unique to the software.</returns>
        public override string[] id()
        {
            return new string[] { "firefox-aurora", "firefox-aurora-" + languageCode.ToLower() };
        }


        /// <summary>
        /// Tries to find the newest version number of Firefox Developer Edition.
        /// </summary>
        /// <returns>Returns a string containing the newest version number on success.
        /// Returns null, if an error occurred.</returns>
        public string determineNewestVersion()
        {
            string url = "https://ftp.mozilla.org/pub/devedition/releases/";

            string htmlContent = null;
            using (var client = new WebClient())
            {
                try
                {
                    htmlContent = client.DownloadString(url);
                }
                catch (Exception ex)
                {
                    logger.Warn("Error while looking for newer Firefox Developer Edition version: " + ex.Message);
                    return null;
                }
                client.Dispose();
            } // using

            // HTML source contains something like "<a href="/pub/devedition/releases/54.0b11/">54.0b11/</a>"
            // for every version. We just collect them all and look for the newest version.
            List<QuartetAurora> versions = new List<QuartetAurora>();
            Regex regEx = new Regex("<a href=\"/pub/devedition/releases/([0-9]+\\.[0-9]+[a-z][0-9]+)/\">([0-9]+\\.[0-9]+[a-z][0-9]+)/</a>");
            MatchCollection matches = regEx.Matches(htmlContent);
            foreach (Match match in matches)
            {
                if (match.Success)
                {
                    versions.Add(new QuartetAurora(match.Groups[1].Value));
                }
            } // foreach
            versions.Sort();
            if (versions.Count > 0)
            {
                return versions[versions.Count - 1].full();
            }
            else
                return null;
        }


        /// <summary>
        /// Tries to get the checksums of the newer version.
        /// </summary>
        /// <returns>Returns a string array containing the checksums for 32 bit an 64 bit (in that order), if successfull.
        /// Returns null, if an error occurred.</returns>
        private string[] determineNewestChecksums(string newerVersion)
        {
            if (string.IsNullOrWhiteSpace(newerVersion))
                return null;
            /* Checksums are found in a file like
             * https://ftp.mozilla.org/pub/devedition/releases/60.0b9/SHA512SUMS
             * Common lines look like
             * "7d2caf5e18....2aa76f2  win64/en-GB/Firefox Setup 60.0b9.exe"
             */

            logger.Debug("Determining newest checksums of Firefox Developer Edition (" + languageCode + ")...");
            string sha512SumsContent = null;
            if (!string.IsNullOrWhiteSpace(checksumsText) && (newerVersion == currentVersion))
            {
                // Use text from earlier request.
                sha512SumsContent = checksumsText;
            }
            else
            {
                // Get file content from Mozilla server.
                string url = "https://ftp.mozilla.org/pub/devedition/releases/" + newerVersion + "/SHA512SUMS";
                using (var client = new WebClient())
                {
                    try
                    {
                        sha512SumsContent = client.DownloadString(url);
                        if (newerVersion == currentVersion)
                        {
                            checksumsText = sha512SumsContent;
                        }
                    }
                    catch (Exception ex)
                    {
                        logger.Warn("Exception occurred while checking for newer"
                            + " version of Firefox Developer Edition (" + languageCode + "): " + ex.Message);
                        return null;
                    }
                    client.Dispose();
                } // using
            } // else
            if (newerVersion == currentVersion)
            {
                if (cs64 == null || cs32 == null)
                {
                    fillChecksumDictionaries();
                }
                if (cs64 != null && cs32 != null && cs32.ContainsKey(languageCode) && cs64.ContainsKey(languageCode))
                {
                    return new string[2] { cs32[languageCode], cs64[languageCode] };
                }
            }
            var sums = new List<string>();
            foreach (var bits in new string[] { "32", "64" })
            {
                // look for line with the correct data
                Regex reChecksum = new Regex("[0-9a-f]{128}  win" + bits + "/" + languageCode.Replace("-", "\\-")
                    + "/Firefox Setup " + Regex.Escape(newerVersion) + "\\.exe");
                Match matchChecksum = reChecksum.Match(sha512SumsContent);
                if (!matchChecksum.Success)
                    return null;
                // checksum is the first 128 characters of the match
                sums.Add(matchChecksum.Value.Substring(0, 128));
            } // foreach
            // return list as array
            return sums.ToArray();
        }


        /// <summary>
        /// Takes the plain text from the checksum file (if already present) and extracts checksums from that file into a dictionary.
        /// </summary>
        private void fillChecksumDictionaries()
        {
            if (!string.IsNullOrWhiteSpace(checksumsText))
            {
                if ((null == cs32) || (cs32.Count == 0))
                {
                    // look for lines with language code and version for 32 bit
                    Regex reChecksum32Bit = new Regex("[0-9a-f]{128}  win32/[a-z]{2,3}(\\-[A-Z]+)?/Firefox Setup " + Regex.Escape(currentVersion) + "\\.exe");
                    cs32 = new SortedDictionary<string, string>();
                    MatchCollection matches = reChecksum32Bit.Matches(checksumsText);
                    for (int i = 0; i < matches.Count; i++)
                    {
                        string language = matches[i].Value.Substring(136).Replace("/Firefox Setup " + currentVersion + ".exe", "");
                        cs32.Add(language, matches[i].Value.Substring(0, 128));
                    } //for
                }

                if ((null == cs64) || (cs64.Count == 0))
                {
                    //look for line with the correct language code and version for 64 bit
                    Regex reChecksum64Bit = new Regex("[0-9a-f]{128}  win64/[a-z]{2,3}(\\-[A-Z]+)?/Firefox Setup " + Regex.Escape(currentVersion) + "\\.exe");
                    cs64 = new SortedDictionary<string, string>();
                    MatchCollection matches = reChecksum64Bit.Matches(checksumsText);
                    for (int i = 0; i < matches.Count; i++)
                    {
                        string language = matches[i].Value.Substring(136).Replace("/Firefox Setup " + currentVersion + ".exe", "");
                        cs64.Add(language, matches[i].Value.Substring(0, 128));
                    } //for
                }
            }
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
            logger.Debug("Searching for newer version of Firefox Developer Edition (" + languageCode + ")...");
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
                // fallback to known information
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
        /// the application cannot be update while it is running.
        /// </summary>
        /// <param name="detected">currently installed / detected software version</param>
        /// <returns>Returns a list of process names that block the upgrade.</returns>
        public override List<string> blockerProcesses(DetectedSoftware detected)
        {
            return new List<string>();
        }


        /// <summary>
        /// language code for the Firefox Developer Edition version
        /// </summary>
        private string languageCode;


        /// <summary>
        /// checksum for the 32 bit installer
        /// </summary>
        private string checksum32Bit;


        /// <summary>
        /// checksum for the 64 bit installer
        /// </summary>
        private string checksum64Bit;


        /// <summary>
        /// static variable that contains the text from the checksums file
        /// </summary>
        private static string checksumsText = null;

        /// <summary>
        /// dictionary of known checksums for 32 bit versions (key: language code; value: checksum)
        /// </summary>
        private static SortedDictionary<string, string> cs32 = null;

        /// <summary>
        /// dictionary of known checksums for 64 bit version (key: language code; value: checksum)
        /// </summary>
        private static SortedDictionary<string, string> cs64 = null;
    } // class
} // namespace
