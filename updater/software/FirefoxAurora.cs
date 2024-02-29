﻿/*
    This file is part of the updater command line interface.
    Copyright (C) 2017, 2018, 2019, 2020, 2021, 2022, 2023, 2024  Dirk Stolle

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
        private static readonly NLog.Logger logger = NLog.LogManager.GetLogger(typeof(FirefoxAurora).FullName);


        /// <summary>
        /// publisher name for signed executables of Firefox Aurora
        /// </summary>
        private const string publisherX509 = "CN=Mozilla Corporation, OU=Firefox Engineering Operations, O=Mozilla Corporation, L=Mountain View, S=California, C=US";


        /// <summary>
        /// expiration date of certificate
        /// </summary>
        private static readonly DateTime certificateExpiration = new(2024, 6, 19, 23, 59, 59, DateTimeKind.Utc);


        /// <summary>
        /// the currently known newest version
        /// </summary>
        private const string currentVersion = "124.0b5";

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
                throw new ArgumentNullException(nameof(langCode), "The language code must not be null, empty or whitespace!");
            }
            languageCode = langCode.Trim();
            var validCodes = validLanguageCodes();
            if (!validCodes.Contains(languageCode))
            {
                logger.Error("The string '" + langCode + "' does not represent a valid language code!");
                throw new ArgumentOutOfRangeException(nameof(langCode), "The string '" + langCode + "' does not represent a valid language code!");
            }
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
            // https://ftp.mozilla.org/pub/devedition/releases/124.0b5/SHA512SUMS
            return new Dictionary<string, string>(101)
            {
                { "ach", "c4ef9443c7a0482ba72f2acb525c5c18b600facb5621fcb9f6f75237cff75285b24929bf195bab917ff785b834d0e78e059943e548b4854c34fb16e3c0fdfb94" },
                { "af", "9d9721a2c1707dc438a550100820fb4ec08c61134d60e48466822fef53bdcda39dc9556abebe1c788af713a96d616bcc2d9786ae8ab3d50f421eb7da8bf0f95c" },
                { "an", "df0472c2ae7a2149377d36e23bced0439d185b050f9f1efcaeba7095fe7712446d3dc3ccfdf0374fd98c9d359441a39a97399b54859d6fc6e6090646d5eaa2b8" },
                { "ar", "b8af3d0fdb091627aa2883e8d5e3c8b8125fe49f160633f3eaa2983b7f146286d71b7e84e1779a915ab9774e565d771562723be629fd67653deec18da9feb79c" },
                { "ast", "6135bfe688e1749b3a90952bf8558ba71dd269000a46f1477ca499cda5c95f88f281f81547c93c52971f8b1d4dfb1447639713aa40ae7a83849bb9a2e847b296" },
                { "az", "1f00d848741278db5fcfb8528be4a5c0457b781fe9b2bc223eeece243a4560637b4b69a2ca9b7930ae219d04f8b49c148bc11fda1c41252925043ae5f80dffa8" },
                { "be", "9826adfb3db0db3482756094e21a27fbe9cda955e8b42e5d458c0fba1bb80bc31af29739e8d5568c83c4c1e8aef02241228506c45c296f2251d88da784e57f02" },
                { "bg", "76d4ff99dfd4c57812af04b7f0896ab44e5b1aa7aaf2686611b0d51a9199c220300f6e5a0bd52183dbf26891436871caa3ab1e53bfdb18ab793edeab99e82cd0" },
                { "bn", "073b07a1aa7f8e85b00e8cb3fa7973dd14fb0bb073bd99d5ff2a8748d7796d69c811704b9c2d54b61d745e1543ca7da02ba328fd171eae4a947ead52b888c36c" },
                { "br", "41b9050d74849abad12d416f13a10f0dea5430f565d7aeaaf9644093620112e83c790c6068d16c6b4694ccd0c569e3789c91dba180d25dd47fb9a650bbc36f25" },
                { "bs", "76f08441d7f6afc7b72f4850a48ab12803d7ce1bcb7590c5fd227019d31b00d93e36958e91c6dad07e236d3ce49f5a2216e9e967716054c9ba3ee00238951e46" },
                { "ca", "168b70a9698f5a32e1623573f08bb8901b93e9e3966a068cc1237826421d3304a55372f48a778fa9ed32a2be87bf2444313bcfe5a647d36be7ab27862967e58a" },
                { "cak", "a6f6d0e07a47433d7f595650e1ddce5f91fb6029481937d97d1c60882bf500b33833765f85dcfb0a364478ad083b2efe1c3543abee8f46c04e5f9d6c8d2469e1" },
                { "cs", "cdf3d61050a3d75e8491f2fade1e364d9ce89a4ec55c79804a14d23d0ae73ec2279844799ef554732fd6dc627eec45eecc94bc288ca025f02f27699cf5b2e649" },
                { "cy", "5f2fa9b1855e3068f1f90e331060afb3314276974bf09ccd7d318fbf0d4d0961d618756a9eb07c2cadc279f116ec9727a9e6d56b4e28218eca4d664c78501747" },
                { "da", "f201fc12ef1957016191a7c279061a30e74ca2e3caaa7b4aade6326243a2f3cab6cc86b3e842d272e7863f0abca1fec7f836d1c79b9da44f91f025abb6836d42" },
                { "de", "badc4dcac685702dcd298ef802c3aa7a043edf7b20821739163a4fa71d74e4151e06ea5c480e6c387d9e384a0a97223283127d550d75bc262a3c2c7c1ed0b3e0" },
                { "dsb", "a07f63bafe8e4b1ef42a6aea28e2465d7e31bc24a780271baf135e76a66289ad614bed3c969bb5019233ee32b39cf80abefdfb403d6feba602ec371a4ff978f1" },
                { "el", "ca527afb24b84043d6eb1efd4539bcdef00cef4902b4c66e38ddb0898f97167369c828a70aaca8a0e0dd1a9be235cc0a91efa6cdf64aa1aa665bc5d9ea6ece40" },
                { "en-CA", "3b3d7c0ce9bf1f30fed1c3f526b62a265c435bf7a0545d75454ad38e41beb40a4676c5f4bb39605b5d496d531891bd701036ded43fed755d7ea7c9368182e479" },
                { "en-GB", "72f63c833df2dfb3d0c5f45890d051c5d808259890529506aad945167ffcb302110509984415edbf4ffce73dd13d3567a2f8050f9646422dab80bc042dc37e64" },
                { "en-US", "467701d1ad9b79439cdcd7ccea9a030b4f5c1cc3bf7a1ee926ff551763028936591a6687349dc6d4b607cafb04b85862abbe2bf03730015294f0d7ad54d87899" },
                { "eo", "e5a7b96b95f3bdd9680fa7b7e96a2edd45a2caede32092a56762566b48acabd6085f79ffebbf386d78faa0f8850e4bfabdbb2d397bd7f34c385d58c8eb13e20f" },
                { "es-AR", "ba39dce9350a5871d0ae2b87eea535b8c319ee628f5ed0e01fa20f99597fb4174906ac7f4ced0218a9d583b93b54d6c4781990121bf853f88be084e95024de00" },
                { "es-CL", "38b4550dedda545231ca1983b960d8a4b0ce8f4708ff3002b20d407070233a2f1c52e614b0827a33c93e73f8fa4c4868c00e5ed711b7a082af1fb0ae396858a5" },
                { "es-ES", "3760c95ede903bf34b3e787da786b4a0e9d68162576550f8876c1ae75ca990ced1d33c0f96b53ee9f1c7d475cf41bf2d9c2f6b47677eccf497bd5b73b8a6a7af" },
                { "es-MX", "e55f36b4a5ac1b01d6b4cf8eef1cb0bea5643f73a80b47a4b97b087238b3a57aa52d076cc93aededf9cbbc36e70bd606c5d117a7f5b0c3923677ccb5c0f6c4b1" },
                { "et", "5e04fe65f79ff56b3491d0e49a5e8188a9a1f56e296bfe796d7f297fd4a88a27b8b4f5f4697595390648ffd02557407dc7d59aa1f1bfa4231e27980c5eb4fec0" },
                { "eu", "64bd18d2d6bd672d89f77be8808e58384f1c2a28820d67f621cde530846f9a8259d93eab0b2eb19c03f2f57392a437a797d46cedd268ea2d7f65fdd9839f5865" },
                { "fa", "8f28708170b35f87ea935b316f44c800ee86f9c3ade5d33d1d0038b90936bd195402517d0860ebd32bad69a0bc88c5314119b9683ec59bd35cd68443dd4f9dcb" },
                { "ff", "35c197af112fe7cf12d00a3048c8cc60665672e79279ec7c2808e912c32316d961975e60685efcb88d89251f1a3f8741beb78edff48a95ca1e4b6920f158f546" },
                { "fi", "3e4d10371d2c7950065844dcbd915ed9d72a2b81af5252a76a168416fa0519c035b998641690eb8b7e808d482deade5843f1e77b975c86071e6438001714ce98" },
                { "fr", "f6ad6540d81ca5052cf7edd10f886dc79db7bf755dbd7ca2e4393e2ab48fa37eb3235bcdacdcc29fef3d4a1ff37d4b385dbfc87322c6ed5901980c116689dbbc" },
                { "fur", "40787c41e0d2f0fbfd4ee624189a5f28cec9a19a451ba2256906ab18354d5dccfffbb6920a38228bd5a0ee406b4fbbc3f115134c3bd5f56ccc3575f8eaec5d64" },
                { "fy-NL", "1d860ac29f5b906c12961929347614dbef6b92c296d9bf67401efd03062adc9592a607ac601d90d0487850d41953a0f55a0f167e41eeea2e8ec1aca4bb20aaf2" },
                { "ga-IE", "11ad198384e42eda2320483f0c05d8872b3646fa00570b2d3778b3cee7a029becb51901e118e20967c2bbd227d8342e3daf665ec3d0d2609e39c56f1a765e982" },
                { "gd", "711404b10724bbb6ee20275e0dbe1a95fe1335ab7ca1950d82910bad86f4d17a06e0c85ba85cd42c5eb97d86a0dc60bac485b40080908faa4b9568abaa583ec1" },
                { "gl", "333c1c7acf3ad22518187d0667a654b2725c3a77e0f26cbe610d1672cda23eb676e05fc10024d321ef30a6bffe0ea05caaf325c5f0048e0360ed3249e8e922c9" },
                { "gn", "c641a5489e833d2d3a76957adaf22a4cd7abb0c21b26f8113166bf6cda00b5945ca8acd8f4a9ffcd9de94ee5fa59f3038c7df72e7c94469e26b2a2cdd4d1a2fa" },
                { "gu-IN", "fc61984110ff171f056b9151f130ae7bd60c387edbd50c9f742dc90990d1a035235f895b1b40882ee826e803e614c0846fd3d19b1ec51cb2476072f4bfa024ef" },
                { "he", "2b7598d7c1161d7fe14a6fd34c5d67185863e5041709cd716277cf2e0e82a697f550570b9c98673fa99ecc93caadf3f4a6c377fcc60d08e26d6349f4092cd1cd" },
                { "hi-IN", "4d7d3bb6ef054e9771dd21af6458fa0ae90c8503ccc0bc70555e3ba676381f15bcc5f233166c73ad6f93e2045750da1561c78a56f0b95cb7621b891e5a7dfe10" },
                { "hr", "c246115494cb2984ca21c56e9ee967ab76fa718397ea55aded30800a9bff81b8eb5118347fc5d3f5b66a6f757d6ccb60b44279298ca69296af865f95fdd7b1e7" },
                { "hsb", "30e4ac8c4fb92b863a41136272739655b21f125ae82172cf6bb9742b8deba61d397432711a18ff30c98b02b20e25bea4b177f077f3b0f8d3cdee494f581cb927" },
                { "hu", "3f159408c3adce7673b41e5cfc2ba73bca538031630b2ad52684936c7bd0be2ce16dc7c9f8e8e7904b83ec270196e9c22318a3fabf696b70d532e82a010c1acb" },
                { "hy-AM", "d752499cab5e548394d3a339d56ee0c8b1b6891b3ef3bb641d9c5596095f77e0e4db35bffc2554f9d470b08836a7984047e370389586b690d6099daba00ebe01" },
                { "ia", "8db05c1133fdeb2b6a03a796d4edfb88d19b3a938ea4df798e65a9261003ec1eb4711eea98be10872ff0aa3d833f5d3d7a2a9be1ac10afeeb300b3a16578eeb1" },
                { "id", "5b6a07e927e8356b608659341ad50cd8f152dc151d8a2092ccd389cca53e0ea1527db19fade9ed30e090b2dc2294c7a5761111122a4cbab2dca2b8f6d249e1bc" },
                { "is", "a0b6b9375e9e012c4ff0ca06ef1512ae3c1ba87ec089c83020d031d8814c48c8175aa03a95a2f7ff35a3a25cd62fb99c129f404e33a48ce7722829108d2b4b59" },
                { "it", "09de3f2eef78fa8a3621b507e6e6040b6eee24c167f73955c1dd9ee2c7d7df11a25287385225d0d61c894fac360925083b6555d18b559ab12be3f8f8c5314dd2" },
                { "ja", "bc67edb567d010fd37afde980318ef3ceb363cb5ccde480555f16e177880bb5b450990c324383df1163dd16f8d81be373d8fba3480f0e5a89a266f707961a953" },
                { "ka", "484be7e7add87e9393c617982cb3f9deef4771170941edba02632f69d1f3cf0dac6b78ee5aafb086460bcd44a3f724ed4ddf3be52dd8c9779bf6cb6939036309" },
                { "kab", "86b4abbc353f0388305fe1b8585337e56fa2e4fbf4965d961763af5e2d106a383211caa4f7225514113c904ca9753ba3b454b5ed2e654433e34c4b2bac0cca91" },
                { "kk", "fd96ff7a90c57bdbf6dbd365177855af0372ff383293d19ec54c10a8710254af6fbc169a201b345fd856e6ffad2e31a501142d47e366bf9d79d1b4344eda92d9" },
                { "km", "23aa515d10a05167a6db91099e451698777d02b35bd1e6c52f30cb93440cf3ab96741e0d963f27301b375190262a15b10f724384ee7d7f9caf36b7742c9dac47" },
                { "kn", "bb3b5656df4c757748cc700211446d5628068cf63a3cf7792e7db498a4e2cbf51b0ad29509cf083a451e9193a0461077730a1bebc19aca652980462d9b421e43" },
                { "ko", "454793f7a60cc710ed3ebd5e780a7e8c4b6480c78495fb725f9a018f9903fa03fcd5063ba29b66c50e5679fd8fe37acfef6de2ecad2293ba2fa886b2c4aa1604" },
                { "lij", "a9c49d2447eaee7006c1cd8286b2eebf41d6744eb26cea131142a97b3e4f452d51f02f1d3809d315b10e74f40df941a9b70f3a8ae691e68cb84d808644219b0d" },
                { "lt", "2067fe712c0b13f1021bd7e2fcc479f83ab55d624e479a0a40579ffef6c3f551772149ba66c0fd1edf37e180a017905f4a51d80c0447629e06837ef5ff7bfd37" },
                { "lv", "b4269a4b1e48258af84fc422710d304d063b3651398572188ce98214823ef84dc4f6922b6512ae9f9de1253c9b1fb1eae2546223b9f7b2d6b8527e8023087506" },
                { "mk", "4975fe2acc8f55eb0dcff55eae824788e6d6533e2cf40152a4cc463fbd1e1f5385e75ce5caf646923db1a4b298b2fcef05d78d5d7be6c5f97e54505dcd7bd17f" },
                { "mr", "1d7bc06b4045ae4f34e3d91e582580b81968684db9fbd1cbc14f3b3ed55cd7d448cde9a7bd9fe4e258a72e58a7330f7c9a2ecc863554fa8cacfef6f0a8ae9600" },
                { "ms", "e276f522f1baf9b5db1000abdb9c3f9324522989314a10afd596212e3715b5347a6f50ab191032008c03f93121ee95bba6f6401d433a67d003fec38bae1dd0c7" },
                { "my", "bdcd2b188dfd24aeb45044d664dbf06573235f1d0eb7d05fc7d4db1c3364df7a06886da94381ed307a8a7b786d60cf493497a371297aff359d32af3a8d4a62d0" },
                { "nb-NO", "d950f23bb94f523c7c631b827480565a8beefc9b8b0b3b6b8c58482d2d808abc0955cc491b391daf827005542dd8679d74a2287f708d0a7265b855dbcad8e774" },
                { "ne-NP", "5e2186a97cf7575bed4603ceb16f340a0a36bfe5c1c2760f210cae7ee920dc395ad20934832bb64512551dd47e6c92f9ec352b2ae23da1b7c5bddf5625a967b2" },
                { "nl", "9a9173e94af797d345fa05925c3a991716ec6674203d80d7d1f1d8779be71c2f0cdf19880d80c24eef4562238a1d35c6bf2ecd1f5c39f3782bbddb032ecae747" },
                { "nn-NO", "fc4dcbee944cb9dd2ea481bd0e00777818054a2cd071e86320a39422de709ae863eb3dccca9771973bfd21a12fdbf40bc5b8437a33d64bf57b7c86dabd90b1d5" },
                { "oc", "6769d29c352f7ed484ff776de1f212ee62abbcc312a7bf506cfc9e4ef664128aa186d513b2f74a9c06613504d8b9d54e7777406551ee10d8d3e2c8bcf3205f65" },
                { "pa-IN", "f1a1635aa27bacda6f596e6080646a682f62389f8324021465277a79a7f8dd9ce04cae2d1e0673fb1662c041300ad2bfea48040ff5bd9e5cd03c67410cfa4370" },
                { "pl", "5a8638bfc59f92de89361f1993b027b9b6e3cec94132f11276969d2d1021c72c63db5d889b01d11c457be119a782fc3bc13d2ab6fbc87acc7bb7388580b861de" },
                { "pt-BR", "edb837638a29d4b40cccba2459f2fc70a87bf5f9717fce97e6a14fcc7b753c8b45937c08c0377aeb7e3590d2abc4bf2cf04ce6ab01ebfe6a2488310016d320b6" },
                { "pt-PT", "be480d744ee1721e2a14e510c63dd94deac23bb682818544828e5a23e96bb26bd777e00745c2214d71a71bcc5ee0610a12d500e49daf150e89b55aa317c2c38c" },
                { "rm", "438210ac63646b93e3e97c35c952b009628533cf0afa667888f594a4d93b03b12c41ac50a01fd9ac1f8f7fe3dbef914e392875200b393a57864acc6eda7e1e65" },
                { "ro", "fe50f29caef6a4a59bde3d78343d31542e6cb2b3baeb4a8f1d4a4bfa6d5125fc8970c4480952dc7dbe48f2fa565e3da32b29dba2a86e2655ef0f9d95e80771b6" },
                { "ru", "21cb8b12daf2705385dc60b81a773c37dce0d25f71719f3680ea930bd985c6eeb41b27d6c34155b7bc8828d689ec453c798f60f16671c4e14db4a8fbc525bf80" },
                { "sat", "eb0c9b26b6c4c8afe04a7ec88ad6691f558cac1a2fc7880f8550ade24ed39d570cc26bb382cc0b8b1ddcfeffce8c45ec66325e38740cac2f456abcefe31e852f" },
                { "sc", "d56d2eeaa2733fdb848764ca2bd8b33702f5922a9198cd65bc69807646490213ca1d7de45b480a2a81a0fdeb6e7cb13854e982c7dd1c51eff297b58a2563e91c" },
                { "sco", "f737461f78d2efe936d2c29f65aa613e370b832af3ace5f95f322fdbc9e49ac173c19df8601bf0c948cf94be26ab20e3469f9eb8b0fd3058a167d00b8d2f60cd" },
                { "si", "1fd370bd628e0dbe3b365f8b1c63c210728c3853cff42a17902526b80a6c85775c2332c7ffdf4457d3211e6f7c00dd484ed065567d9118c873e33f6f80d716cf" },
                { "sk", "0dfe241c92e4c11e3d33017a1e0bd4ce2ac48e5fbfe14039e4878d18fe03ca15815a846cdaf861639f8015daf5cf55cd6f413ab5b4c6af3694f981cdeec9ca8b" },
                { "sl", "ab88e0dd89c1cc15754d50266164be3de970008906ec0c44d1cf1dccbcf6ef46fb4b116ca9c67a63f3be2522ca4bbd4ef9ee68f6eb79db0ca8da5cf6e7fe3ebc" },
                { "son", "101315faa61e92c09e402c6efa92f04f5ec3b9d961cd0fbdfa8a41f29a783d19376303f1e0388c53b7d7fa0d2b037e8187775dde0213fceddd2d304c446966df" },
                { "sq", "6fe2a64c2bf8b1cd64715b123f7b6e13cf2b492e40f4085f66dd52e2d64aa28b95e57a0fb3244635131fb60157fe538c310b3089feb0fd23fa93f298ed5c350a" },
                { "sr", "7ddebc9a057f8875475bbe96f4f1ab0362ba2c7bd79b0444b2e524fc2d66317b7055b898a0dca8b23f7dbe8770d36dc3d08307430bbb8455e9c9cbd587f824ee" },
                { "sv-SE", "9770d8aa0a8624dfe1cbff347fe2e45d6a189507919424085678797277e5fb54828484f57fea2bc8d172631fe3ff8a8164e28d0b37650c374dc9a0f310ae6d4a" },
                { "szl", "7d65d8c0aaf9f65ed6161526ba639d857f735686f01e5e1b1f620b3760136532cdba81a19d17845afc03ff794377767f0b3966808cd6ef7bcdea684d684796e9" },
                { "ta", "acda552e904ada2c8272e48984c710fb110e78987b443a576381b786604fa6c7f68edaf174acb84ffdc4dc7b0b61d818802e4046618cb0ed2c6574c1c8345a08" },
                { "te", "f3ef772a667cd7f805b3a8bfd8a56d2602eafb667b6ded18b241f7d03cfbc391e2cc41d00f62631c527732cf6a8387097dcdc0ea5ad01fa7c367fd9efba3f208" },
                { "tg", "4fa36a519315f2044d11e17a8f758869342839bffb17f3e80d35ecf6c79b8b2ece54507c9ff7ea94a4ee0a9cd1dfb4f416e40f4c2140ea66c585ed30163eabbb" },
                { "th", "528f287ed0715a5d2645a9525ec7efff4a6579fbd69e8a5af929d61114fe8057e89805cf7702df0043056b5a3e9baf75fdc0644bbd98f18a07638dde7963fe2d" },
                { "tl", "d11ac917e1c0564b33731e9da1da3bc90f92aba602d8dab800f33cabb9d1329d639c6a2997aff576d3c8b3e7983ae5dccee6147c9fd9a94bdf6ad1e3ba5dac17" },
                { "tr", "a45a95ef144e2f6890ba8663e1b5f644aa9aa22aecfc24b020e7d9191ddcb00a0d9e8af326c74510c305cf9ea3ec157d46163acb64abb1c4250f3262f9aff7e2" },
                { "trs", "6eb2974a7ef422ed9bb989bc2aed34d79ebb1be8214d8fe43e92160964a261c3d08be9450dc1095761d998e893505b6d7075774344495ff874eb8a9642384391" },
                { "uk", "bd08bf7faca8f664bd20c71f263a170c26c5370af67b1c171100489d550209b7ef8e9b7316cf902c78690f48ab18dff1b914d498d065be751ad3430cbcb56645" },
                { "ur", "bb8cd33831628ac7af4fb8dd5d869603fd81f4f8fdfdfc82104c4ddbc7e0f52dca0c5bb9b397b9616409a6fe6f709e457fe47260342eaa03ee3f20cf72f894d3" },
                { "uz", "34edcf8b1c9c45b1f7a1a373b19e5be59c23cf4c37163aef6fb7793b4900ba0f6feb0038f517db2319b4621492e584ad87cee706bfdb84289bbbca050ef76645" },
                { "vi", "d1aeee5fa532a10d19b8e8ff795762cb4b0490750b7aa1ff59739124d1a73312000e58f0f3ffb54b2bf1df3e148cfac01d3b2268b8f329adb19c8e18e38306ed" },
                { "xh", "c132c17a37f94af20ced977943ca1c166512d2397eb1b62390735181d472a7bea70e37242b6c42611d51b3d922f542ac014bff4b92e4497cf9af31ad9a2a1adc" },
                { "zh-CN", "a73687bdbb7bd25780f593dabab38a55939ae773b978c28bbdb15b92b7eba391b05a79d75692208e7680e02982cad078de9515e89cb43b9ed76c78707d6a1360" },
                { "zh-TW", "bb3a53966cdbea147502c29345f7cc6163346ff47328d85cdb981da2de3a0f4d017a94777dda7086f8bdb30b3cd22f97e34da642050ce8f1fd672d4b07f53063" }
            };
        }

        /// <summary>
        /// Gets a dictionary with the known checksums for the installers (key: language, value: checksum).
        /// </summary>
        /// <returns>Returns a dictionary where keys are the language codes and values are the associated checksums.</returns>
        private static Dictionary<string, string> knownChecksums64Bit()
        {
            // These are the checksums for Windows 64 bit installers from
            // https://ftp.mozilla.org/pub/devedition/releases/124.0b5/SHA512SUMS
            return new Dictionary<string, string>(101)
            {
                { "ach", "4362180b15b1d36f4c12ee3f531c0db4352b3a7f4b18e4468b51a18f3db6f9160a237825f43c3631904eeed4fe3574f2f79c77916844a302f4fc5c0fecb7d0bc" },
                { "af", "1b5aabcd858f685090a96c38d0999cd63a5e18b3aaa70346f6fd95a72578f07f9cf3677c1f535a957528d93b8ecb48d17090511c6b7cc6fb235c47e19caff2b3" },
                { "an", "c9a6207e94d7556af81fe8e8f016a388f20fe2bbbdd74958717aaf83936035f87ddb4facd0b2ba2531e8483aeedf7865578a82c12a1628897cbfd865ea2d2b7f" },
                { "ar", "26946d14f12ecfb0dbfcf06e8bfee6dbc02e07ead007fd523ba262479e4485e35b5c7f81326f485886f220fc0e0ecb58f1d511f1c2f308862c74ccc57ffd5648" },
                { "ast", "41c78c14652a05f5ff3eafc91991ee657775ba88c5b7ca692ed462693714e8275f1c7d7c27ff23a4f68ac9e3b910c412db8fe8d6290886651957a87608750864" },
                { "az", "3aa1be2ef48e9fca3dfcb0cd85c9e24a1401b76bcbc9897c2a1f36b533b6d98e2b3ef1077e9f3967ba7d876c5162888e4a839ab2b005f715ab13ebb8c3d5554e" },
                { "be", "7f046eda14c2f50ad697831dfeaf8eab0b7ade01acce6f940b407bb7b1060b15e8ebec87e5fb6579e47918ceac9a6c63448ae7c7933406a058035bc99da32074" },
                { "bg", "85ada966fad14ffd49dafeb80c64efaa7d8d908b715999f5700eea72137062373366a048630647fb222788bd0fd059f8a88ded44ed475a1254efa2ec2546a454" },
                { "bn", "08d6a4c31611aed9753cdeb5f949d96bf9ef3218fa10dfe39ee6f736a00250b15ffa143605688bf03b712752db2059c96617921f17ab0c4cd7b334d7473c6512" },
                { "br", "9bf207b45d53cd46b0b1128268d1c3673c7291e80b6668255a845d844a41f9f132b8b578f55a9b9a459c20ec6a8e3f0de75fb9ea0d31c968393799eabd7408a6" },
                { "bs", "2a6050e06d8b1014460d0835d3193fa23027b2bd7a4891c6537de40e7a0d027cc139aeb053176ca7d89ac1feddfe400594aa7614fe691cc3674bd0509e947c06" },
                { "ca", "a1b4a6701dec7dfd02a2909be92299d805d90c21336f76450f843723f6989919f09298d019c4a79f69462aa9316d1a8b75d8a252dcd9271c314a36000e8f6dce" },
                { "cak", "c26409e7c04abf17cbf62b20e57f8998fab02be7447febfba79ba41101fa423897978981ea5a6bd0ddcd4ecbfe75328e3a5c4a8462642f741e52e68df082a294" },
                { "cs", "c5ab7623707d7d5f33b95ed53189cdc2f528e53cf6fac50bcfef16e7b0b340acf47860f114826ca0adc6cc283962ebdbda090cfcc6047a0d2cbf0d8bb98bd924" },
                { "cy", "27bea544faa54f08f6cab1dc5d31513dccee62f2568de26250e6f6189b09e07986b95e23ba8ab505c722724e025ea3c5621972406ec6a3d89b70b35b34420610" },
                { "da", "ae8cb736ae153dbcfc7d1631e2f0d4bdbc63138ccca716410c5bd4071879302fe9801ed586d8b9fc9f5f6003461bbf8d4eb7796bb998527beaf69ad4db1a555d" },
                { "de", "1d4253012f42c474f53ff2088c9affbba2fbb931ae71dc7ebd380312c399dee8a5ccbc2e5b5dc66591c4dbdb6e3b14952cf617650d13da137e3a425278beeccc" },
                { "dsb", "2cc6a81ef80fc2b95b8d367f69fdd152e0ecf6a90cc6e30c06a89be0ed7e458047bf3a868dceb2ace27c35f76bb7eb69f0088a8e996e86174a6ce21f308fca11" },
                { "el", "4a127c5b97132ec529d43e08465db458363665903400578feba0efa22e2379c64f5f153d63aea14adfb496a38effdbce210fb32d40d8b05480c534cc9438b521" },
                { "en-CA", "507d6e971972f2d1cadc37d6c2ea4291bcaf5988f237ed8033b85a2a4e0dc69d79f8f048799b211614d08a367b3bdac4b61f767c35128ea3204824be694bd814" },
                { "en-GB", "e18bb93ea97fb51fbb13479a3a469af1b511043c093433c4c11e95a750e55ab31ac8c5ab7fab2328133d2cfe001d73cb68cface486aee1625caf1b1ab78fbd0d" },
                { "en-US", "fe61229371c257623bf00a300042caa3aea4d42a1fb2ad53f4043de67fe98f114d7deac18b473ebda42a6612e98104e416668d2145b5270bfc7c27167017dfa0" },
                { "eo", "04c4029700621c7538500e3bc38cd10790fd724000cc207069c42e199a908a68440dbfc5c2439f5f2ab191c6dcfd8d1d5b68aab99eca67bd460302cf3441b558" },
                { "es-AR", "1a31e1b3ad2e8beeade6a9972925a97184ca65b03160b9f9277aeed4e2bb304d4e44fdbe20765428910a3bada9dfcf36abcc517a0f9670f463cabdcfabddc23d" },
                { "es-CL", "59b05d1d6ac311ea7e5c6b319ca854e7010c19ffd4e303af001ec00fbc54a73aa99512d874c4121849872e68d4aa7e01acc471d58c08b548385efbc1cd63a044" },
                { "es-ES", "9f23479236b9bd3d38daa08aa729d5b17ccfc7786b48caf114ef411915f5134092a6bedc47782a72ab913c75108e1012904160433d8b91098342f74f9c5d9d1e" },
                { "es-MX", "bf60a9b458cd767673f4536cd8f73f47fbcc36625bd4df4231b2f3dd1261ca666ea79f66c88810aa42572b6e118f55e74f0a5f88ce7cfac1dab1ec4ee681172e" },
                { "et", "f4e9a61d2b888fe9dbbef492961a87b493377103fdb760825ab15ee10eddf266c794aa1f10136dc4837c918ca5733f56b1f807f9429507ec3990fac5e7d1fdbb" },
                { "eu", "203029c165bb98dc3cbb75bae76addb0b3c18db5cf9059865200483e06ae66d74735a40cb99801430ac7c339ce14e580bcd80de396d32c4aba122589005aafcf" },
                { "fa", "501efb823c96beda538ae43a64f335beae7df6ba5e67cb5b089ca71053dfb2e1da7f2d8d9d0736aadd0aa6e9e849c474810c57d3f7995defd1ea2e18390ba031" },
                { "ff", "fee6edd018ef4d0f05ea1e83106dc101cf5b674ce82fe749535fed063317a688219d0fcd4e4eb44834801b1cd2adb656e61b37a558b9f8e17cc77c9a219ed9e4" },
                { "fi", "a4efdf3d68206a68446299f42097b45f5181abb91c4ce976b2ba2bbb8fa15febe6075956b6b1e3a5c4ad2cb9087215d3371c1f344aea6a21294e133c33672f4e" },
                { "fr", "afc0a8f1e73b78bb0b205be42353828003e43b992db7bce26a04c1f7c4fc81f2538d7452a8baaa99e61105e7141951f5bdb1a0ee6ae5563f18550b3b83c74438" },
                { "fur", "2b8438feee68f100633f209eb50676787b7b7d91bde915c67a6ad5a926d48bfafd7c1942b9f9736449cd252e2612707f1b40952065d8c4d13f98be32daa17b6d" },
                { "fy-NL", "7fa6ed6223b2bafbe61bcc57440b334e7f515861b50b8c6300eba213c45f75a58fcd21a0ed2a02b0ff636e63632533c1d7b1ee32917f8301986e540c55406da3" },
                { "ga-IE", "3e550d534d7e70b3132247a6640a8054e1ac0e3cdb3ad3abf12b0a736494ffa18bbf95cfd16883eb8f7d35bc5f1fe6f6c78c1745d8ba58cf7e37b558734c4283" },
                { "gd", "4c9925ecd72a4e71b021219e6d0f86f6e82b98cfa086c221495330fc52d36e6248c766315c4c05517617811fc4d1a968360e6543be29754d81ac8a25e5462158" },
                { "gl", "c35015a596123b4b15404f83e16b921d63fbb26ac57c203f694d207ffcaf5af319abab68e13a2538fb6de02ae73cb16955911071c750b23ad44d70a0b9a4eb96" },
                { "gn", "c37e03b71f9dcffdc50da7d8a84f476518a334947e94a515164fcb595cd6060e906300b4d4f0b7133fdfda6bb7a68b0d3daef38ea58ddfb0a2c04058794986d3" },
                { "gu-IN", "b00d4f3cf3fc464f89471d740e3fa89fd34bf0b7e13848f1b140535829c46698123d6b7eab80bff92042c8abff9b29aeb098dae7708ec01eea093aa0c0048797" },
                { "he", "bffd429479775c9d33d8a5fdca68936af95a9af2debb63321d56ff9c3dda70255a0022459579dd0bc8097b141d33fd35d04384683b7109305ffdec5360ecf38b" },
                { "hi-IN", "835c52899322a9838c8daea8772f22e1bdeaeb419d3493f6d0b683b357fff8682ab5fd7950b55a209a087f2a4dce86ab1c81ffd0425a967f74f12c3c76203d7f" },
                { "hr", "350ef0d94752b20bde9d01a4678d1c767c5a157f4c1af0d8f39cfb3ac12c4eb65c3d6f2ea02e54f1de7682f5268d483f54a06064f737a418682a4114a765bad5" },
                { "hsb", "dc02660ec63e7afcabd95ac5d65868122cd9325329477ca453e50711489effd5f67544de203b5cc399bb35a2de044fc602b25f4bbef4c8026fe0d4150c137f96" },
                { "hu", "9fd505e274675849cb3d0e81f8c4f1ce9683159f0ccf72ea65969c9e123d2360f4a45af5e7883c2b0709ac8b58055b5b596732514697a4c636653fc1d9a07392" },
                { "hy-AM", "a3e5fddc6f465bca4152bbe49856843f3025830090fa2683c2a94bde98f61cfd7715017af8d16bd7f878bb98b0d15c6e6aa934896eb63aa29f7d20bcc26fccfd" },
                { "ia", "ab8bdd5db091e01a5369eefc85c6680edddc3818ce62b6c1ec24980bef2ed06b32b7344b6facbfff6554ac798e3ab4c73ba6c521dbebe6a57eb67666f971d449" },
                { "id", "af36534145840a6418846daa2070b8c2a0ae60f80ddd7660a88a1446a79cc3e5028a7e23b85d747865a2972b527e58ff72517a1e1704b1bebb9c2a896677dace" },
                { "is", "3b4586fa0bb5ee5aca5f719bdce7f5963dcfd2dd7525ad3e9778cac6bd180d41137d6666a9ec34420c2c4cba5b9594144c9b0c5190d924196defe10106e88c35" },
                { "it", "fb0a5bf3d18c0531a8f9fc9fa6f79dd89c7e92268cbf94ec7931e995bccaf3d3281a106dbccb99cb45e61cef8a972a94723140de3e9e8a74d682d796ac3cd5a0" },
                { "ja", "6c50bed54007ab648b5099184397ea3d5d6eb260d54699eca4e39f14df15ef0309d1bfc70afbe46da7f60c87471dac6aa6fcfee529820ea309f594665ee9f473" },
                { "ka", "34be2561a7f49742cb03c15c7af02216a949d8e6737b9cc22d3f66de2bfd944f9e5d3b56f7ec64f05096ccb428d3721abd93dfff308a95e1db86f56c26b97bc1" },
                { "kab", "29b4e9fafb9ca72b180d6e95d779c15fb2525177b4379ec05c68f9f1064cf37c48ca06e4a8d22828d202b99281a6995fb54805c5427f16acd7216795b0334ced" },
                { "kk", "ebd4bc88852b56a9281a6524928e04d979f3b84c13cc9b5c8157b2167a2c6fad7933bc61d15646d644459900ff91a7a7b7f6545b382bbb9d8a413a2fbd7f4e67" },
                { "km", "0ee8eedc7a9285dbc1c067f87cc281bd91ca7677583cb31899570856c6a4247886e1b4c018c443cc5b233280dbceb55f64b2db1c8d95b2622123b38b3ee43511" },
                { "kn", "f044de2a010a70886007071cb1929370c3ca91c9bc56b69965c8138ad18ba9b40034a56aa3009e2d3edde3eb036f65eb8bd358c86745fb32878f083a07530df6" },
                { "ko", "397196d5c79bf6731e73919954b9c89f1028379d8ff39f98a906ae0753a2da170f799e71a43dc30c46cd7f80c79df6667a21c1aae13183624e1cda896b761a45" },
                { "lij", "9d31ca52cd0aac75285f764667afb1bcab8dc9c75647131ec4d69831a3c21fe5e361bb0c5212b6409877e280bd43a5a74f21bb4bf0c30825f6db1f53526c0a4b" },
                { "lt", "f69b94a1c24e7622ce612d8857a1fa47d562522e02fd578a40b09602b5fdb996710c8e818f0f8f36cf0a221dedf1c89e515bf946c8435a216f5bd4295e8e9237" },
                { "lv", "77396fec811d45301fe7ac79123fa2b69e8e58281e0175adefa2ad0c4e66d540e39addf05698bd9598d0c941ce71ccca0ce966de49f26541557da699cefad5f3" },
                { "mk", "0850ade412e8b3b7a5d14dce50a887f5746d92fd0e4f3b759308dc3702b411aad56b9098c1c15569f0dcb1f81804523e7cdfcf92ff155c1f8a30d276e9e7c069" },
                { "mr", "1067e3dccb54bb6533e5663443aa9a96127fbad98434f29da4bf402f83917bd65ffc0a270e4087f1b5e1cd91ebcfaee5cd58e665562b31fa879e7ce26f7159fc" },
                { "ms", "f4988f58cdcaabd107ea66a479ada372a4fcdc746a5084ea54b438150528efe64edce489fc85f5dc5094605b15c935ceee990e1cfc11826d1383e580d5b3974f" },
                { "my", "88be43937271f393637258f693ee013c58781a765a24d0b68869edd0b2db4dabc2835c2b59ef98a4ed184b30498b1743b7bba0cd3152a644f70f9853363385ae" },
                { "nb-NO", "21270c710e7a25bf3639d1db1f45018f47216b7ed91189bd5b41fd9bf6489fff160d5ed2af4a8b240359b131664ee8625aa08b9ed9f2dbcb7ec83783950c6e92" },
                { "ne-NP", "d909193668fc9464b2d439a43d0958c50bc6df114be9c479a50f4244e086a699e4eb1e60645da9fd2193966fbc86efe2fc7b99e8a8441549fce79278af983011" },
                { "nl", "fdcb33fa9cb309454268a596890b06b33e233c8ea28fd33055fdf559bfe8cb55941874ee72c4e0ec945e16ffac49c764716c2c26c7a9d122bdb6b8b7b883469b" },
                { "nn-NO", "75801c0be299c2d3dbfc0db47a574cd25c8012f924e73b58ba66cbcb4b7849dd2f6b667a37a2fa7878d98e4af896d1d2cf6c9509500001dc776288efe863160d" },
                { "oc", "2b2e75082e8e4511ac5e9fa7402c25962b99cd640ae4ea017ef1e54a6244c1c5b23bc3a842b8a22c741bf13776a6b7cf7a7c8729fd17b838084d8e58671bd683" },
                { "pa-IN", "6b03fc69c21e159512ab2d52408786e5abb9beb67b333ea4d4e06a3f98c01b797b68a86c17e29226e9ecd0981b015916da6f92db7e3f155bedc5788c4b9a1f18" },
                { "pl", "23b165defe1ab9947e9d4cbcc91bdf1f973c9fbcea2470ac65fc942fb5173dfb99fdb46bd343ba47c9f9b03f85ce5f15b3a26822c4124779dfc361b385899fe3" },
                { "pt-BR", "cacc36b46cdeec377d425e79848040a94eb51a4bf059328cd9e206a0d50be9ee8345492e1f92eddb3e021f2421a68b22a7611e26f6fb99367568af156cd91866" },
                { "pt-PT", "98762eba3b91ab3b67e50df2a94057be10e3fd3525e01eccf7a2addfa022eda90c1d95b768613c38217bba5b1d423928eab4cde960cdb849efdbfcec90a2e810" },
                { "rm", "a50fe9a31759b6a8c7dffb00628687ebe3dc30670b7387c5436b433a1ea873e4c803a7a0c5a4e732181e88315c48e8456df67e5fb3bc8dca646a59a2a72c60d4" },
                { "ro", "885b09a614dfe3773eb4baed88af6a99a46e84baf38b2507c227d384a8f832ba83de8a72f1bcbf8911fa24565e9cbbfadd2fad042518ac2f8a83735cc3297b86" },
                { "ru", "a0f43c4b06291ce5712904f11c18ee8c40e1543aca5a826bd6cbae8abd7221291d7effeedb59785d797688499cd6797f313560605f9558c316e5a5f8d7fd605d" },
                { "sat", "0f6b42c5e489729fa98eda78756a06b58b0b0d02a6bc2a6e4ed0c511e9db515e71cf08c31bbdf08018b9fec6aebcda4d6b9aea4495571f8d960a5f7e20bf64b6" },
                { "sc", "39e4bd5c3775fadb5d339a0c251088e7e67347840fc0a97b0af820feeefa048ae9557e8f9024c65781d6b4061f266896d234b60563bb481f57baa016e66f24e9" },
                { "sco", "80c93cf37bdda28a5a211f96bbc4bec3e8c8b0113f50917572966f903d3cbcc0a04d1f25df1bbec9d127035befdf25caaad600c537bc76c4ed8edcf30dfe1387" },
                { "si", "8ade13ded48977b64968fbec5d6eb4ac98b8b87e92abbbdb492ef9aefccff4ce6f87fd2488e285a89ef007d6d94fd403a658095e2907446f5387706328b5d037" },
                { "sk", "487c33f67d1d0c12127b8918af1b748263ccebe313426696adc232cfcef6f5f321752fd96069176a2a75cd0cd796645d9ef64c87d20e5cb906677d92469f220a" },
                { "sl", "782a3d7d387ed329ee44fe664a1a190128ff56a78c4b03379a2e29367f42a7f56bf67b091742d059b24a7e4a6c550316cff0b2fea908e335c45a71fb53436498" },
                { "son", "c0929061b13daa10b359c233250527e45107eda10afca6abc06da16c642085b6f2a1c7cf79097566f179bb4eb3807e85b3ace1452755748e578a01df1f0be9ad" },
                { "sq", "22ddeeaccce3ae3f81138873993754bbc73cc1b1f9fe799b901748848336ef695e3725693368dec9d7c8cd3e8666a5fc3b79a4ace63f1352cc356a57302448ab" },
                { "sr", "a3cac62267b46aaa2c915cc825e9ac9c2b7f85ff61a1130c1609f33d6192d373a56018e2ab206519474e874a1241b0e4b8c572f53a470b632e4d41dd391939a4" },
                { "sv-SE", "02a6558d3b0850ce0b3e6d1fa4aa6764eeef5a310fd8bcac5d9513eb27a337118d2860e5355020ae9fde2b48e114e3ea559f23da67e45880f16a417c30d6b5f0" },
                { "szl", "ef178b6cc5969c4273e62adf9db4f3c7e6b3ebbcbef8c5453161bc2d06ec89be525ce26f657a67187a2729397198718e27366d70269cd2e9e8776b69e52e2e13" },
                { "ta", "aad36ef952205e3ee13c568f722cb9ac32d9324f1e593f600c77e4b1de1bf8a4e15347435368cc8ee2cd9909599a6b9ce2e97308d992fd57a1afa670db264435" },
                { "te", "87d2d90ac6af7bbee0d8be1e0202c3306df6bc76404f58b7d730bfc8efe29f4f8545ef6dd77860138679a56db1e9e235a822c1b507bf90ebf1f8da5e09e7b2cb" },
                { "tg", "8d56adeb6311cd0effd327dce8261b8e111cf79ae7cffe5e45f441f853ecaf9a09f9da083a5e72263a40cb6dc834425689035d4ed23775199af2e02bdafafff6" },
                { "th", "256f9109e681d17b9136b7be8954d223d7b521f23e5186adff3e095e7c491d046e18a47e1d250bf7f47901f94f4f384580f62507723417599091b4fa8a003589" },
                { "tl", "d19be68af51417e0f01931f726fa45faf34c1481bd07b22e87a190967311916dd8cbe740089f19cad55b17fb530c35a43570422207f0441b90c479ee2de21a7c" },
                { "tr", "d517af3db8940775baf2a749c2bfdda6426e72aaf1208bae59026543fa007cdcaec19bc73f477250fc3b87f8bb78742c25a7d8c76a10bf69661583afc0c00489" },
                { "trs", "b2af549a05070eed4f5fa3954bab33739757841ff2d1953bba31db66b447f58090783625d76bde81fb450187f9a431f8bf6cb3c6bcd02d1d2c5867073f7d1a02" },
                { "uk", "ffe53ad87b393470f3c07fe73e9e1cabd586fa9048384d23a5d7b93b9d6c917d21ba758da11006f44cf9bd446115b4ebd37deaba6c8ee3c0fed5bedfde2946ce" },
                { "ur", "55d2677279d0cf5882ff00dc3a99f150ba6b292c1cb82be290eca1d326efe58ee78cb2f162f40be8a8d3a379e7145de7d5ed9b709ceac29074e915899a3be2b3" },
                { "uz", "a243bb864e399569201081cf74980eb8edd8979af18865941301e29bef90e314e286b33d08f52ca6937deb8c7806e55beb1b14b98f09078fdbde781e7c92e0b7" },
                { "vi", "6bd4082fa0c945418487a238d6da546ab51ccbb27009ba9ce5d0ed9ff6906ffa5cf5f0203d99bea64655e1d7afe2208cda0c66f2a083bfcaca21f7cf27610c90" },
                { "xh", "286438ec7abca1c2f1947e6fdc2ce3e8eb24ec7bd75e811f2b109650439e5e3a9df090d6cea2d0985123379465265989529b20664a4db5d8157a8ebe63dbb092" },
                { "zh-CN", "411093a875760540e106af2279fe69ad059353b20dcf2b0ff4b134f3fe71c8da27d4fbb9e24ffa0009d3384138a7e39cb126d7a19ada5a9091443877b2478ff4" },
                { "zh-TW", "880bcc8dcde3912bb77cbb40aed6574e75f4266f5cae1ef41ae043ab6b0c1ec347326238a2a5cf1c3b2cccecc15fbba61655fbeb56887cb90dbfeddfa1228775" }
            };
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
            var signature = new Signature(publisherX509, certificateExpiration);
            return new AvailableSoftware("Firefox Developer Edition (" + languageCode + ")",
                currentVersion,
                "^Firefox Developer Edition( [0-9]{2}\\.[0-9]([a-z][0-9])?)? \\(x86 " + Regex.Escape(languageCode) + "\\)$",
                "^Firefox Developer Edition( [0-9]{2}\\.[0-9]([a-z][0-9])?)? \\(x64 " + Regex.Escape(languageCode) + "\\)$",
                // 32 bit installer
                new InstallInfoExe(
                    // URL is formed like "https://ftp.mozilla.org/pub/devedition/releases/60.0b9/win32/en-GB/Firefox%20Setup%2060.0b9.exe".
                    "https://ftp.mozilla.org/pub/devedition/releases/" + currentVersion + "/win32/" + languageCode + "/Firefox%20Setup%20" + currentVersion + ".exe",
                    HashAlgorithm.SHA512,
                    checksum32Bit,
                    signature,
                    "-ms -ma"),
                // 64 bit installer
                new InstallInfoExe(
                    // URL is formed like "https://ftp.mozilla.org/pub/devedition/releases/60.0b9/win64/en-GB/Firefox%20Setup%2060.0b9.exe".
                    "https://ftp.mozilla.org/pub/devedition/releases/" + currentVersion + "/win64/" + languageCode + "/Firefox%20Setup%20" + currentVersion + ".exe",
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
            return new string[] { "firefox-aurora", "firefox-aurora-" + languageCode.ToLower() };
        }


        /// <summary>
        /// Tries to find the newest version number of Firefox Developer Edition.
        /// </summary>
        /// <returns>Returns a string containing the newest version number on success.
        /// Returns null, if an error occurred.</returns>
        public static string determineNewestVersion()
        {
            string url = "https://ftp.mozilla.org/pub/devedition/releases/";

            string htmlContent;
            var client = HttpClientProvider.Provide();
            try
            {
                var task = client.GetStringAsync(url);
                task.Wait();
                htmlContent = task.Result;
            }
            catch (Exception ex)
            {
                logger.Warn("Error while looking for newer Firefox Developer Edition version: " + ex.Message);
                return null;
            }

            // HTML source contains something like "<a href="/pub/devedition/releases/54.0b11/">54.0b11/</a>"
            // for every version. We just collect them all and look for the newest version.
            var versions = new List<QuartetAurora>();
            var regEx = new Regex("<a href=\"/pub/devedition/releases/([0-9]+\\.[0-9]+[a-z][0-9]+)/\">([0-9]+\\.[0-9]+[a-z][0-9]+)/</a>");
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
        /// <returns>Returns a string array containing the checksums for 32 bit and 64 bit (in that order), if successful.
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
            string sha512SumsContent;
            if (!string.IsNullOrWhiteSpace(checksumsText) && (newerVersion == currentVersion))
            {
                // Use text from earlier request.
                sha512SumsContent = checksumsText;
            }
            else
            {
                // Get file content from Mozilla server.
                string url = "https://ftp.mozilla.org/pub/devedition/releases/" + newerVersion + "/SHA512SUMS";
                var client = HttpClientProvider.Provide();
                try
                {
                    var task = client.GetStringAsync(url);
                    task.Wait();
                    sha512SumsContent = task.Result;
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
                var reChecksum = new Regex("[0-9a-f]{128}  win" + bits + "/" + languageCode.Replace("-", "\\-")
                    + "/Firefox Setup " + Regex.Escape(newerVersion) + "\\.exe");
                Match matchChecksum = reChecksum.Match(sha512SumsContent);
                if (!matchChecksum.Success)
                    return null;
                // checksum is the first 128 characters of the match
                sums.Add(matchChecksum.Value[..128]);
            } // foreach
            // return list as array
            return sums.ToArray();
        }


        /// <summary>
        /// Takes the plain text from the checksum file (if already present) and extracts checksums from that file into a dictionary.
        /// </summary>
        private static void fillChecksumDictionaries()
        {
            if (!string.IsNullOrWhiteSpace(checksumsText))
            {
                if ((null == cs32) || (cs32.Count == 0))
                {
                    // look for lines with language code and version for 32 bit
                    var reChecksum32Bit = new Regex("[0-9a-f]{128}  win32/[a-z]{2,3}(\\-[A-Z]+)?/Firefox Setup " + Regex.Escape(currentVersion) + "\\.exe");
                    cs32 = new SortedDictionary<string, string>();
                    MatchCollection matches = reChecksum32Bit.Matches(checksumsText);
                    for (int i = 0; i < matches.Count; i++)
                    {
                        string language = matches[i].Value[136..].Replace("/Firefox Setup " + currentVersion + ".exe", "");
                        cs32.Add(language, matches[i].Value[..128]);
                    }
                }

                if ((null == cs64) || (cs64.Count == 0))
                {
                    // look for line with the correct language code and version for 64 bit
                    var reChecksum64Bit = new Regex("[0-9a-f]{128}  win64/[a-z]{2,3}(\\-[A-Z]+)?/Firefox Setup " + Regex.Escape(currentVersion) + "\\.exe");
                    cs64 = new SortedDictionary<string, string>();
                    MatchCollection matches = reChecksum64Bit.Matches(checksumsText);
                    for (int i = 0; i < matches.Count; i++)
                    {
                        string language = matches[i].Value[136..].Replace("/Firefox Setup " + currentVersion + ".exe", "");
                        cs64.Add(language, matches[i].Value[..128]);
                    }
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
            logger.Info("Searching for newer version of Firefox Developer Edition (" + languageCode + ")...");
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
        /// the application cannot be updated while it is running.
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
        private readonly string languageCode;


        /// <summary>
        /// checksum for the 32 bit installer
        /// </summary>
        private readonly string checksum32Bit;


        /// <summary>
        /// checksum for the 64 bit installer
        /// </summary>
        private readonly string checksum64Bit;


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
